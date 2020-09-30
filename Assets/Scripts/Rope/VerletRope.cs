using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class VerletRope : MonoBehaviour
{
    LineRenderer ropeRenderer;
    public List<VerletRopeNode> ropeNodes = new List<VerletRopeNode>();
    private List<Vector3> ropeNodePositions;
    Transform connectedTrans;
    private float maxRestLength;

    private float RestLength
    {
        get
        {
            return ropeJoint.linearLimit.limit;
        }
        set
        {
            SoftJointLimit linearLimit = ropeJoint.linearLimit;
            linearLimit.limit = value;
            ropeJoint.linearLimit = linearLimit;
        }
    }

    public float Spring
    {
        get
        {
            return ropeJoint.linearLimitSpring.spring;
        }
        set
        {
            SoftJointLimitSpring linearLimitSpring = ropeJoint.linearLimitSpring;
            linearLimitSpring.spring = value;
            ropeJoint.linearLimitSpring = linearLimitSpring;
        }
    }

    public float Damper
    {
        get
        {
            return ropeJoint.linearLimitSpring.damper;
        }
        set
        {
            SoftJointLimitSpring linearLimitSpring = ropeJoint.linearLimitSpring;
            linearLimitSpring.damper = value;
            ropeJoint.linearLimitSpring = linearLimitSpring;
        }
    }

    ConfigurableJoint ropeJoint;

    private void Awake()
    {
        ropeRenderer = gameObject.GetComponent<LineRenderer>();
        if (!ropeRenderer)
        {
            ropeRenderer = gameObject.AddComponent<LineRenderer>();
            ropeRenderer.widthMultiplier = 0.05f;
        }
    }

    public void BuildRope(Transform connectedTrans, int numSegments, float maxRestLength, Material ropeMat)
    {
        float distance = Vector3.Distance(transform.position, connectedTrans.position);
        float constraintLength = distance / numSegments;
        Vector3 direction = connectedTrans.position - transform.position;
        for (int i = 1; i < numSegments; i++)
        {
            Vector3 pos = Vector3.Lerp(transform.position, connectedTrans.position, 
                i * constraintLength / distance);
            GameObject ropeNodeGO = new GameObject();
            ropeNodeGO.transform.position = pos;
            VerletRopeNode ropeNode = ropeNodeGO.AddComponent<VerletRopeNode>();
            ropeNode.previousPosition = pos;
            ropeNodes.Add(ropeNode);
        }
        this.maxRestLength = maxRestLength;
        ropeRenderer.material = ropeMat;

        this.connectedTrans = connectedTrans;
        Rigidbody sourceBody = GetNonKinematicRigidbodyInParent(transform);
        Debug.Log(sourceBody);
        Rigidbody connectedBody = GetNonKinematicRigidbodyInParent(connectedTrans);
        Debug.Log(connectedBody);
        ropeJoint = sourceBody.gameObject.AddComponent<ConfigurableJoint>();
        ropeJoint.autoConfigureConnectedAnchor = false;
        //ropeJoint.anchor = sourceBody.transform.InverseTransformPoint(transform.position);
        ropeJoint.anchor = Vector3.zero;
        if (connectedBody)
        {
            ropeJoint.connectedBody = connectedBody;
            //ropeJoint.connectedAnchor = connectedBody.transform.InverseTransformPoint(connectedTrans.position);
            ropeJoint.connectedAnchor = Vector3.zero;
        }
        else
        {
            /* Assume that this is a stationary transform */
            ropeJoint.connectedAnchor = connectedTrans.position;
        }
        ropeJoint.xMotion = ConfigurableJointMotion.Limited;
        ropeJoint.yMotion = ConfigurableJointMotion.Limited;
        ropeJoint.zMotion = ConfigurableJointMotion.Limited;

        RestLength = distance;
    }

    Rigidbody GetNonKinematicRigidbodyInParent(Transform trans)
    {
        Rigidbody currentRb = trans.GetComponent<Rigidbody>();
        while (currentRb && currentRb.isKinematic)
        {
            currentRb = currentRb.transform.parent.GetComponent<Rigidbody>();
        }
        return currentRb;
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
        float constraintLength = RestLength / ((ropeNodes.Count - 1) + 2);

        VerletRopeNode n0 = this.ropeNodes[0];
        Vector3 d1 = n0.transform.position - transform.position;
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
        d1 = connectedTrans.position - nLast.transform.position;
        d2 = d1.magnitude;
        d3 = (d2 - constraintLength) / d2;
        nLast.transform.position += 1.0f * d1 * d3;
    }

    private void DrawRope()
    {
        if (connectedTrans != null)
        {
            ropeNodePositions = new List<Vector3>();
            ropeNodePositions.Add(transform.position);
            for (int i = 0; i < ropeNodes.Count; i++)
            {
                ropeNodePositions.Add(ropeNodes[i].transform.position);
            }
            ropeNodePositions.Add(connectedTrans.position);
            ropeRenderer.positionCount = ropeNodePositions.Count;
            ropeRenderer.SetPositions(ropeNodePositions.ToArray());
        }
    }

    /* This can be used for increasing the rest length of the rope. For example, increasing distance
       between two objects */
    public void IncreaseRestLength(float amount)
    {
        RestLength = Mathf.Min(RestLength + amount, maxRestLength);
    }

    /* This can be used for decreasing the rest length of the rope. For example, when we want
       to pull two objects closer together */
    public void DecreaseRestLength(float amount)
    {
        /* Setting minimum to 0.1f removes jittering */
        RestLength = Mathf.Max(RestLength - amount, 0.1f);
    }

    private void OnDestroy()
    {
        Destroy(ropeJoint);
        for(int i = 0; i < ropeNodes.Count; i++)
        {
            Destroy(ropeNodes[i].gameObject);
        }
    }
}
