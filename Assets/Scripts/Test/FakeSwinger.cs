using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeSwinger : MonoBehaviour
{
    public GameObject fakePhysicsAnchor;
    Rigidbody rb;
    float constraintDistance;
    Vector3 velocity;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        constraintDistance = Vector3.Distance(transform.position, fakePhysicsAnchor.transform.position);
    }

    private void FixedUpdate()
    {
        Vector3 acceleration = Physics.gravity;

        Vector3 testPosition = transform.position + velocity * Time.fixedDeltaTime;
        Vector3 toAnchor = fakePhysicsAnchor.transform.position - testPosition;
        if (toAnchor.magnitude > constraintDistance )
        {
            // we're past the end of our rope
            // pull the avatar back in.
            testPosition = fakePhysicsAnchor.transform.position + constraintDistance * -toAnchor.normalized;
        }

        velocity = (testPosition - transform.position) / Time.fixedDeltaTime + 
            acceleration * Time.fixedDeltaTime;
        rb.MovePosition(testPosition);
    }
}
