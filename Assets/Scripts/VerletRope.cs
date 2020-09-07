using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerletRope : MonoBehaviour
{
    LineRenderer ropeRenderer;
    public List<VerletRopeNode> ropeNodes = new List<VerletRopeNode>();
    public float totalLength;
    private Vector3[] ropeNodePositions;
    float constraintLength;

    private void Awake()
    {
        ropeRenderer = gameObject.AddComponent<LineRenderer>();
        ropeRenderer.material = new Material(Shader.Find("Unlit/Color"));
        ropeRenderer.material.color = Color.blue;
        ropeRenderer.widthMultiplier = 0.05f;
    }

    // Update is called once per frame
    void Update()
    {
        DrawRope();
    }

    private void FixedUpdate()
    {
        Simulate();

        // Higher iteration results in stiffer ropes and stable simulation
        for (int i = 0; i < 80; i++)
        {
            ApplyConstraint();

            // Playing around with adjusting collisions at intervals - still stable when iterations are skipped
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
            

            /*
            // Get the current distance between rope nodes
            float currentDistance = (node1.transform.position - node2.transform.position).magnitude;
            float difference = Mathf.Abs(currentDistance - NodeDistance);
            Vector2 direction = Vector2.zero;

            // determine what direction we need to adjust our nodes
            if (currentDistance > NodeDistance)
            {
                direction = (node1.transform.position - node2.transform.position).normalized;
            }
            else if (currentDistance < NodeDistance)
            {
                direction = (node2.transform.position - node1.transform.position).normalized;
            }

            // calculate the movement vector
            Vector3 movement = direction * difference;

            // apply correction
            node1.transform.position -= (movement * 0.5f);
            node2.transform.position += (movement * 0.5f);
            */
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
}
