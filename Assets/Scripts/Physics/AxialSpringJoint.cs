using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxialSpringJoint : MonoBehaviour
{
    Rigidbody rb;

    public float Ck = 0.1f;

    public float Cd = 0.1f;

    public Vector3 axis;
    
    public Vector3 connectedAnchor;

    public float restDistance;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        /*
         * Force required to reach x0 with velocity v0 in one frame is:
         * f = -(m / dt^2) * (x - x0) - (m / dt) * (v - v0)
         * 
         */
        Ck = Mathf.Clamp(Ck, 0, 1);
        Cd = Mathf.Clamp(Cd, 0, 1);
        axis.Normalize();

        float m = rb.mass;
        float v = Vector3.Dot(rb.velocity, axis);
        float x = Vector3.Dot(transform.position - connectedAnchor, axis) - restDistance;
        float dt = Time.fixedDeltaTime;
        Vector3 f = (-(m * Ck / (dt * dt)) * x - (m * Cd / dt) * v) * axis.normalized;
        rb.AddForce(f);
    }
}
