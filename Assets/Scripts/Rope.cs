using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    LineRenderer ropeRenderer;
    public Rigidbody anchor;
    public Rigidbody load;
    public float restLength;
    public float frac;
    public float lastLength;
    
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
            Vector3 disp = anchor.transform.position - load.transform.position;
            float length = disp.magnitude;
            bool isTaut = length >= frac * restLength;
            //Debug.Log("Taut length: " + length.ToString("F4") + "|| Length: " + length.ToString("F4"));
            if (isTaut)
            {
                float delta = (lastLength - length)/ lastLength;
                /* updatedSegmentRestLength = frac * restLength - delta * frac * restLength */
                /*                          = frac * restLength * (1 - delta)               */
                /* frac = updatedSegmentRestLength / restLength = frac * (1 - delta)        */
                frac *= 1 - delta;
                lastLength = length;

                Vector3 ropeDir = disp.normalized;
                Vector3 loadVelAlongRopeDir = Vector3.Project(load.velocity, ropeDir);
                Vector3 anchorVelAlongRopeDir = Vector3.Project(anchor.velocity, ropeDir);
                float k = 200.0f * frac;
                //float b = 20.0f;
                //float b = Mathf.Sqrt(4 * load.mass * k);
                
                Vector3 f1 = k * (length - frac * restLength) * ropeDir;// + b * (Vector3.zero - loadVelAlongRopeDir);
                Vector3 f2 = -k * (length - frac * restLength) * ropeDir;// + b * (Vector3.zero - anchorVelAlongRopeDir);

                if (pullIn)
                {
                    Vector3 fPull = -1000.0f * ropeDir;
                    f1 += fPull;
                    f2 += -fPull;
                }
                else if (release)
                {
                    Vector3 fPull = -1000.0f * ropeDir;
                    f1 = Mathf.Max(0.0f, f1.magnitude - fPull.magnitude) * ropeDir;
                    f2 = Mathf.Max(0.0f, f2.magnitude + fPull.magnitude) * ropeDir;
                }
                load.AddForce(f1, ForceMode.Force);
                anchor.AddForce(f2, ForceMode.Force);
            }
            else
            {
                //Do inverse kinematics maybe
                if (pullIn)
                {
                    frac = Mathf.Max(0.0f, frac - Time.deltaTime);
                }
            }
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
        if (load)
        {
            Vector3 disp = anchor.transform.position - load.transform.position;
            float length = disp.magnitude;
            if (length >= frac * restLength)
            {
                Vector3 ropeDir = disp.normalized;
                float b = 1.0f;
                Vector3 fPull = -1000.0f * ropeDir;// - b * ;
                anchor.AddForce(fPull, ForceMode.Force);
                load.AddForce(-fPull, ForceMode.Force);
            }
            else
            {
                //If the rope is not taught, simply shorten length of rope
                tautLength = Mathf.Max(0.0f, tautLength - 10.0f * Time.deltaTime);
            }
        }
    }

    public void Release()
    {
        if (load)
        {
            Vector3 disp = anchor.transform.position - load.transform.position;
            float length = disp.magnitude;

            Vector3 f1 = k * (length - frac * restLength) * ropeDir;
            Vector3 fRelease = 40
            Mathf.Min(f1.magnitude, fRelease.magnitude);
            
            tautLength = Mathf.Max(tautLength, length);
            tautLength = 15.0f;
        }
    }

    public float CurrentLength()
    {
        return Vector3.Distance(anchor.transform.position, load.transform.position);
    }
}
