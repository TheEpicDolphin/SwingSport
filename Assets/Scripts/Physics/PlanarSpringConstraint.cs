using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanarSpringConstraint : MonoBehaviour
{
    Rigidbody rb;

    public float Ck = 0.1f;

    public float Cd = 0.1f;

    public Vector3 normal;
    
    public Vector3 planePoint;

    public float distance;
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
        normal.Normalize();

        float m = rb.mass;
        float v = Vector3.Dot(rb.velocity, normal);
        float x = Vector3.Dot(transform.position - planePoint, normal) - distance;
        float dt = Time.fixedDeltaTime;
        Vector3 f = (-(m * Ck / (dt * dt)) * x - (m * Cd / dt) * v) * normal.normalized;
        rb.AddForce(f);
    }
}
