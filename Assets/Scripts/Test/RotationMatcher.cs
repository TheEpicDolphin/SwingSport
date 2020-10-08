using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationMatcher : MonoBehaviour
{

    public Transform animTarget;
    Rigidbody boneRb;
    // Start is called before the first frame update
    void Start()
    {
        boneRb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        float Ck = 0.5f;
        float Cd = 0.5f;

        Vector3 thetaForward = Vector3.Cross(transform.forward, animTarget.forward);
        Quaternion forwardMatchingRot = Quaternion.FromToRotation(transform.forward, animTarget.forward);
        Vector3 transformedUp = forwardMatchingRot * transform.up;
        Vector3 thetaUp = Vector3.Cross(transformedUp, animTarget.up);
        float dt = Time.fixedDeltaTime;
        Vector3 I = boneRb.inertiaTensor;
        Vector3 wForward = Vector3.Project(boneRb.angularVelocity, thetaForward.normalized);
        Vector3 wUp = Vector3.Project(boneRb.angularVelocity, thetaUp.normalized);
        Vector3 forwardMatchingTorque = (1 / (dt * dt)) * Ck * Vector3.Scale(I, thetaForward)
                                    - (1 / dt) * Cd * Vector3.Scale(I, wForward);
        Vector3 upMatchingTorque = (1 / (dt * dt)) * Ck * Vector3.Scale(I, thetaUp) 
                                    - (1 / dt) * Cd * Vector3.Scale(I, wUp);
        boneRb.AddTorque(forwardMatchingTorque);
        boneRb.AddTorque(upMatchingTorque);
    }

    private void Update()
    {
        Debug.DrawRay(transform.position, 2.0f * transform.forward, Color.blue, 0.0f, false);
        Debug.DrawRay(transform.position, 2.0f * transform.up, Color.green, 0.0f, false);

    }
}
