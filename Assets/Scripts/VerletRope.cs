using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerletRope : MonoBehaviour
{
    LineRenderer ropeRenderer;
    public List<VerletRopeNode> ropeNodes = new List<VerletRopeNode>();
    public float maxRestLength = 30.0f;
    private Vector3[] ropeNodePositions;
    private float restLength;

    //public GameObject s;
    //public GameObject e;

    private void Awake()
    {
        ropeRenderer = gameObject.AddComponent<LineRenderer>();
        ropeRenderer.material = new Material(Shader.Find("Unlit/Color"));
        ropeRenderer.material.color = Color.blue;
        ropeRenderer.widthMultiplier = 0.05f;
    }

    private void Start()
    {
        //BuildRope(s, e, 3);
    }

    public void BuildRope(GameObject start, GameObject end, int numSegments)
    {
        VerletRopeNode startRopeNode = start.AddComponent<VerletRopeNode>();
        VerletRopeNode endRopeNode = end.AddComponent<VerletRopeNode>();
        restLength = Vector3.Distance(start.transform.position, end.transform.position);
        float constraintLength = restLength / numSegments;
        Vector3 direction = end.transform.position - start.transform.position;
        startRopeNode.previousPosition = start.transform.position;

        startRopeNode.isFixed = true;
        ropeNodes.Add(startRopeNode);
        for (int i = 1; i < numSegments; i++)
        {
            Vector3 pos = Vector3.Lerp(start.transform.position, end.transform.position, 
                i * constraintLength / restLength);
            GameObject ropeNodeGO = new GameObject();
            ropeNodeGO.transform.position = pos;
            VerletRopeNode ropeNode = ropeNodeGO.AddComponent<VerletRopeNode>();
            ropeNode.previousPosition = pos;
            ropeNode.isFixed = false;
            ropeNodes.Add(ropeNode);
        }
        endRopeNode.previousPosition = end.transform.position;
        endRopeNode.isFixed = true;
        ropeNodes.Add(endRopeNode);
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
            ApplyConstraint();

            /* Playing around with adjusting collisions at intervals - still stable when iterations are skipped */
            if (i % 2 == 1)
            {
                //AdjustCollisions();
            }
        }

        if(ropeNodes.Count >= 2)
        {
            /* Apply forces */
            Transform startTrans = ropeNodes[0].transform;
            Transform endTrans = ropeNodes[ropeNodes.Count - 1].transform;
            Rigidbody startRb = startTrans.GetComponentInParent<Rigidbody>();
            //Rigidbody endRb = endTrans.GetComponentInParent<Rigidbody>();
            Rigidbody endRb = null;

            Vector3 toHookVector = endTrans.position - startTrans.position;
            Vector3 directionToHook = toHookVector.normalized;

            if (startRb && endRb)
            {
                
            }
            else if (startRb)
            {
                float k = 500.0f;
                /* Critically damped */
                float b = Mathf.Sqrt(4 * startRb.mass * k);
                /* Treating rope like a spring */
                Vector3 fSpring = -k * (restLength * directionToHook - toHookVector)
                                    + b * (Vector3.zero - Vector3.Project(startRb.velocity, directionToHook));
                startRb.AddForce(fSpring);
            }
            else if (endRb)
            {
                float k = 500.0f;
                /* Critically damped */
                float b = Mathf.Sqrt(4 * endRb.mass * k);
                /* Treating rope like a spring */
                Vector3 fSpring = k * (restLength * directionToHook - toHookVector)
                                    + b * (Vector3.zero - Vector3.Project(endRb.velocity, directionToHook));
                endRb.AddForce(fSpring);
            }
        }
    }

    private void Simulate()
    {
        // step each node in rope
        for (int i = 0; i < ropeNodes.Count; i++)
        {
            if (ropeNodes[i].isFixed)
            {
                continue;
            }
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

    private void ApplyConstraint()
    {
        float constraintLength = restLength / ropeNodes.Count - 1;
        for (int i = 0; i < ropeNodes.Count - 1; i++)
        {
            VerletRopeNode node1 = this.ropeNodes[i];
            VerletRopeNode node2 = this.ropeNodes[i + 1];

            Vector3 x1 = node1.transform.position;
            Vector3 x2 = node2.transform.position;
            Vector3 d1 = x2 - x1;
            float d2 = d1.magnitude;
            float d3 = (d2 - constraintLength) / d2;

            if (node1.isFixed)
            {
                node2.transform.position = x2 - 1.0f * d1 * d3;
            }
            else if (node2.isFixed)
            {
                node1.transform.position = x1 + 1.0f * d1 * d3;
            }
            else
            {
                node1.transform.position = x1 + 0.5f * d1 * d3;
                node2.transform.position = x2 - 0.5f * d1 * d3;
            }
        }
    }

    private void DrawRope()
    {
        ropeNodePositions = new Vector3[ropeNodes.Count];
        for (int i = 0; i < ropeNodes.Count; i++)
        {
            ropeNodePositions[i] = ropeNodes[i].transform.position;
        }
        ropeRenderer.positionCount = ropeNodes.Count;
        ropeRenderer.SetPositions(ropeNodePositions);
    }

    //TODO: make rope specifically for connecting two non-rope entities
    
    public void IncreaseRestLength(float amount)
    {
        restLength = Mathf.Min(restLength + amount, maxRestLength);
    }

    public void DecreaseRestLength(float amount)
    {
        restLength = Mathf.Max(restLength - amount, 0.0f);
    }


    private void OnDestroy()
    {
        Destroy(ropeRenderer);
        for(int i = 1; i < ropeNodes.Count - 1; i++)
        {
            Destroy(ropeNodes[i].gameObject);
        }
        Destroy(gameObject);
    }
}
