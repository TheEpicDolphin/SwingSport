using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void RopeTensionDelegate(Vector3 force);

public class VerletRope : MonoBehaviour
{
    LineRenderer ropeRenderer;
    public List<VerletRopeNode> ropeNodes = new List<VerletRopeNode>();
    private List<Vector3> ropeNodePositions;
    private float restLength;
    private float maxRestLength = 30.0f;

    public GameObject start;
    public GameObject end;

    public RopeTensionDelegate startTensionDelegate;
    public RopeTensionDelegate endTensionDelegate;

    private void Awake()
    {
        ropeRenderer = gameObject.AddComponent<LineRenderer>();
        ropeRenderer.widthMultiplier = 0.05f;
    }

    private void Start()
    {
        //BuildRope(s, e, 3);
    }

    public void BuildRope(GameObject start, GameObject end, int numSegments, float maxRestLength, Material ropeMat)
    {
        this.start = start;
        this.end = end;
        restLength = Vector3.Distance(start.transform.position, end.transform.position);
        float constraintLength = restLength / numSegments;
        Vector3 direction = end.transform.position - start.transform.position;
        for (int i = 1; i < numSegments; i++)
        {
            Vector3 pos = Vector3.Lerp(start.transform.position, end.transform.position, 
                i * constraintLength / restLength);
            GameObject ropeNodeGO = new GameObject();
            ropeNodeGO.transform.position = pos;
            VerletRopeNode ropeNode = ropeNodeGO.AddComponent<VerletRopeNode>();
            ropeNode.previousPosition = pos;
            ropeNodes.Add(ropeNode);
        }
        this.maxRestLength = maxRestLength;
        ropeRenderer.material = ropeMat;
    }

    // Update is called once per frame
    void Update()
    {
        DrawRope();
    }

    private void FixedUpdate()
    {
        Simulate();

        /* Higher iteration results in stiffer ropes and stable simulation */
        for (int i = 0; i < 80; i++)
        {
            ApplyConstraints();

            /* Playing around with adjusting collisions at intervals - still stable when iterations are skipped */
            if (i % 2 == 1)
            {
                //AdjustCollisions();
            }
        }

        if(start && end)
        {
            /* Get rigidbodies that belongs to the ancestors of start and end. The rope will apply
               forces to these rigidbodies, if they exist */
            Rigidbody startRb = start.GetComponentInParent<Rigidbody>();
            //Rigidbody endRb = end.GetComponentInParent<Rigidbody>();
            /* I set this to null for now because none of the blocks have rigidbodies */
            Rigidbody endRb = null;

            Vector3 startToEndVector = end.transform.position - start.transform.position;
            Vector3 startToEndDirection = startToEndVector.normalized;

            if (startRb && endRb)
            {
                // TODO: If both start and end are attached to rigidbodies, we will have to go back
                // to college classical mechanics to model this 2-mass spring system
            }
            else if (startRb)
            {
                /* endRb is null. This means that end does not have a rigidbody on any of its 
                   ancestors. We treat it as though it has infinite mass */

                Vector3 fSpring = Vector3.zero;
                if (startToEndVector.magnitude > restLength)
                {
                    /* We only apply a restoring force when the length between the start and end
                       is greater than the rest length of the rope. Ropes only pull you, never push you */
                    float k = 500.0f;
                    /* Critically damped */
                    float b = Mathf.Sqrt(4 * startRb.mass * k);
                    /* Treating rope like a spring */
                    fSpring = -k * (restLength * startToEndDirection - startToEndVector)
                                        + b * (Vector3.zero - Vector3.Project(startRb.velocity, startToEndDirection));
                    startRb.AddForce(fSpring);
                }
                startTensionDelegate?.Invoke(fSpring);
            }
            else if (endRb)
            {
                /* startRb is null. This means that end does not have a rigidbody on any of its 
                   ancestors. We treat it as though it has infinite mass */

                Vector3 fSpring = Vector3.zero;
                if (startToEndVector.magnitude > restLength)
                {
                    /* We only apply a restoring force when the length between the start and end
                       is greater than the rest length of the rope. Ropes only pull you, never push you */
                    float k = 500.0f;
                    /* Critically damped */
                    float b = Mathf.Sqrt(4 * startRb.mass * k);
                    /* Treating rope like a spring */
                    fSpring = k * (restLength * startToEndDirection - startToEndVector)
                                        + b * (Vector3.zero - Vector3.Project(startRb.velocity, startToEndDirection));
                    endRb.AddForce(fSpring);
                }
                endTensionDelegate?.Invoke(fSpring);
            }
        }
    }

    private void Simulate()
    {
        // step each node in rope
        for (int i = 0; i < ropeNodes.Count; i++)
        {
            // derive the velocity from previous frame
            Vector3 velocity = ropeNodes[i].transform.position - ropeNodes[i].previousPosition;
            ropeNodes[i].previousPosition = ropeNodes[i].transform.position;

            // calculate new position
            Vector3 newPos = ropeNodes[i].transform.position + velocity;
            newPos += Physics.gravity * Time.fixedDeltaTime * Time.fixedDeltaTime;
            /*
            Vector3 direction = RopeNodes[i].transform.position - newPos;
            // cast ray towards this position to check for a collision
            int result = -1;
            result = Physics2D.CircleCast(RopeNodes[i].transform.position, RopeNodes[i].transform.localScale.x / 2f, -direction.normalized, ContactFilter, RaycastHitBuffer, direction.magnitude);

            if (result > 0)
            {
                for (int n = 0; n < result; n++)
                {
                    if (RaycastHitBuffer[n].collider.gameObject.layer == 9)
                    {
                        Vector2 collidercenter = new Vector2(RaycastHitBuffer[n].collider.transform.position.x, RaycastHitBuffer[n].collider.transform.position.y);
                        Vector2 collisionDirection = RaycastHitBuffer[n].point - collidercenter;
                        // adjusts the position based on a circle collider
                        Vector2 hitPos = collidercenter + collisionDirection.normalized * (RaycastHitBuffer[n].collider.transform.localScale.x / 2f + RopeNodes[i].transform.localScale.x / 2f);
                        newPos = hitPos;
                        break;              //Just assuming a single collision to simplify the model
                    }
                }
            }
            */
            ropeNodes[i].transform.position = newPos;
        }
    }

    private void ApplyConstraints()
    {
        float constraintLength = restLength / ((ropeNodes.Count - 1) + 2);

        VerletRopeNode n0 = this.ropeNodes[0];
        Vector3 d1 = n0.transform.position - start.transform.position;
        float d2 = d1.magnitude;
        float d3 = (d2 - constraintLength) / d2;
        n0.transform.position += -1.0f * d1 * d3;

        for (int i = 0; i < ropeNodes.Count - 1; i++)
        {
            VerletRopeNode node1 = this.ropeNodes[i];
            VerletRopeNode node2 = this.ropeNodes[i + 1];

            Vector3 x1 = node1.transform.position;
            Vector3 x2 = node2.transform.position;
            d1 = x2 - x1;
            d2 = d1.magnitude;
            d3 = (d2 - constraintLength) / d2;
            node1.transform.position = x1 + 0.5f * d1 * d3;
            node2.transform.position = x2 - 0.5f * d1 * d3;
        }

        VerletRopeNode nLast = this.ropeNodes[this.ropeNodes.Count - 1];
        d1 = end.transform.position - nLast.transform.position;
        d2 = d1.magnitude;
        d3 = (d2 - constraintLength) / d2;
        nLast.transform.position += 1.0f * d1 * d3;
    }

    private void DrawRope()
    {
        ropeNodePositions = new List<Vector3>();
        ropeNodePositions.Add(start.transform.position);
        for (int i = 0; i < ropeNodes.Count; i++)
        {
            ropeNodePositions.Add(ropeNodes[i].transform.position);
        }
        ropeNodePositions.Add(end.transform.position);
        ropeRenderer.positionCount = ropeNodePositions.Count;
        ropeRenderer.SetPositions(ropeNodePositions.ToArray());
    }

    /* This can be used for increasing the rest length of the rope. For example, increasing distance
       between two objects */
    public void IncreaseRestLength(float amount)
    {
        restLength = Mathf.Min(restLength + amount, maxRestLength);
    }

    /* This can be used for decreasing the rest length of the rope. For example, when we want
       to pull two objects closer together */
    public void DecreaseRestLength(float amount)
    {
        restLength = Mathf.Max(restLength - amount, 0.0f);
    }


    private void OnDestroy()
    {
        Destroy(ropeRenderer);
        for(int i = 0; i < ropeNodes.Count; i++)
        {
            Destroy(ropeNodes[i].gameObject);
        }
    }
}
