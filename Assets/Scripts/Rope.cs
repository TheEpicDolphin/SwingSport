using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    LineRenderer ropeRenderer;
    public Rigidbody anchor;
    public Rigidbody load;
    public float restLength;
    float frac;
    float lastLength;
    bool pullIn = false;
    bool release = false;
    
    // Start is called before the first frame update
    void Start()
    {
        anchor = GetComponent<Rigidbody>();
        ropeRenderer = gameObject.AddComponent<LineRenderer>();
        ropeRenderer.material = new Material(Shader.Find("Unlit/Color"));
        ropeRenderer.material.color = Color.red;
        ropeRenderer.widthMultiplier = 0.05f;
    }

    // Update is called once per frame
    void Update()
    {
        //Can do raycasts here to check if anything collides with rope
        DrawRope();
    }

    private void FixedUpdate()
    {
        if (load)
        {
            Vector3 load2Anchor = anchor.transform.position - load.transform.position;
            float length = load2Anchor.magnitude;
            bool isTaut = (length >= frac * restLength);
            if (isTaut)
            {
                float delta = (lastLength - length)/ lastLength;
                /* updatedSegmentRestLength = frac * restLength - delta * frac * restLength */
                /*                          = frac * restLength * (1 - delta)               */
                /* frac = updatedSegmentRestLength / restLength = frac * (1 - delta)        */
                frac *= 1 - delta;
                lastLength = length;

                Vector3 load2AnchorDir = load2Anchor.normalized;
                Vector3 loadVelAlongRopeDir = Vector3.Project(load.velocity, load2AnchorDir);
                Vector3 anchorVelAlongRopeDir = Vector3.Project(anchor.velocity, load2AnchorDir);
                float k = 200.0f * frac;
                //float b = 20.0f;
                //float b = Mathf.Sqrt(4 * load.mass * k);
                
                Vector3 fLoad = k * (length - frac * restLength) * load2AnchorDir;// + b * (Vector3.zero - loadVelAlongRopeDir);
                Vector3 fAnchor = -k * (length - frac * restLength) * load2AnchorDir;// + b * (Vector3.zero - anchorVelAlongRopeDir);

                if (pullIn)
                {
                    Vector3 fPull = 1000.0f * load2AnchorDir;
                    fLoad += fPull;
                    fAnchor += -fPull;
                }
                else if (release)
                {
                    Vector3 fPull = 1000.0f * load2AnchorDir;
                    fLoad = Mathf.Max(0.0f, Vector3.Dot(fLoad - fPull, load2AnchorDir)) * load2AnchorDir;
                    fAnchor = Mathf.Max(0.0f, Vector3.Dot(fAnchor + fPull, load2AnchorDir)) * load2AnchorDir;
                }
                load.AddForce(fLoad, ForceMode.Force);
                anchor.AddForce(fAnchor, ForceMode.Force);
            }
            else
            {
                if (pullIn)
                {
                    frac = Mathf.Max(0.0f, frac - Time.deltaTime);
                }
                //Do inverse kinematics maybe
            }
            pullIn = false;
            release = false;
        }
    }

    void DrawRope()
    {
        if (load)
        {
            List<Vector3> ropeVerts = new List<Vector3>();
            ropeVerts.Add(anchor.transform.position);
            ropeVerts.Add(load.transform.position);
            ropeRenderer.positionCount = ropeVerts.Count;
            ropeRenderer.SetPositions(ropeVerts.ToArray());
        }
    }

    public void PullIn()
    {
        pullIn = true;
    }

    public void Release()
    {
        release = true;
    }

    public float CurrentLength()
    {
        return Vector3.Distance(anchor.transform.position, load.transform.position);
    }
}
