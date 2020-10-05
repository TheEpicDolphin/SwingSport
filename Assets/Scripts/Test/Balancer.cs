using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balancer : MonoBehaviour
{
    float spring = 1000.0f;
    float damper = 100.0f;
    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        /*
         * f = -(m / dt) * (x - x0) - (m / dt) * (v - v0)
         * This is the 
         * 
         */

        float Ck = 0.1f;
        float Cd = 0.1f;
        float m = rb.mass;
        float vy = rb.velocity.y;
        float y = transform.position.y;
        float dt = Time.fixedDeltaTime;
        Vector3 f = (-(m * Ck / (dt * dt)) * y - (m * Cd / dt) * vy) * Vector3.up;
        rb.AddForce(f);
        //Vector3 balancingForce = (-spring * (transform.position.y - 0.0f) - damper * (rb.velocity.y)) * Vector3.up;
        //rb.AddForce(balancingForce);
    }
}
