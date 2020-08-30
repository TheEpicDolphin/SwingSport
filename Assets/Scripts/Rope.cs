using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    LineRenderer ropeRenderer;
    public Rigidbody anchor;
    public Rigidbody load;
    public float tautLength;
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
            //Debug.Log("Taut length: " + length.ToString("F4") + "|| Length: " + length.ToString("F4"));
            if (length >= tautLength)
            {
                Vector3 ropeDir = disp.normalized;
                Vector3 velAlongRopeDir = Vector3.Project(load.velocity - anchor.velocity, -ropeDir);
                float k = 200.0f;
                float b = Mathf.Sqrt(4 * load.mass * k);
                Debug.Log(b);
                Vector3 f = -k * (tautLength - length) * ropeDir + b * (Vector3.zero - velAlongRopeDir);
                //Prevent unrealistic forces by clamping to range
                //f = Mathf.Clamp(f.magnitude, 0, 250.0f) * f.normalized;
                load.AddForce(f, ForceMode.Force);
                anchor.AddForce(-f, ForceMode.Force);
            }
            else
            {
                //Do inverse kinematics maybe
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
            if (length >= tautLength)
            {
                Vector3 ropeDir = disp.normalized;
                float b = 1.0f;
                Vector3 f = -100.0f * ropeDir;// - b * ;
                anchor.AddForce(f, ForceMode.Force);
                load.AddForce(-f, ForceMode.Force);
                tautLength = Mathf.Min(tautLength, length);
                //Debug.Log("Taut length: " + tautLength.ToString("F4"));
                //Debug.Log("length: " + length.ToString("F4"));
            }
            else
            {
                //If the rope is not taught, simply shorten length of rope
                tautLength = Mathf.Max(0.0f, tautLength - Time.deltaTime);
            }
        }
    }

    public void Release()
    {
        if (load)
        {
            Vector3 disp = anchor.transform.position - load.transform.position;
            float length = disp.magnitude;
            tautLength = Mathf.Max(tautLength, length);
        }
    }

    public float CurrentLength()
    {
        return Vector3.Distance(anchor.transform.position, load.transform.position);
    }
}
