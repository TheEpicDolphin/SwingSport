using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerletParticle : RopeNode
{
    public Vector3 previousPosition;

    public void Simulate()
    {
        // derive the velocity from previous frame
        Vector3 velocity = transform.position - previousPosition;
        previousPosition = transform.position;

        // calculate new position
        Vector3 newPos = transform.position + velocity;
        newPos += Physics.gravity * Time.fixedDeltaTime * Time.fixedDeltaTime;
        transform.position = newPos;
    }

    public static void ApplyConstraint(VerletParticle vp1, VerletParticle vp2)
    {
        float constraintLength = vp2.ropeLocation - vp1.ropeLocation;
        Vector3 x1 = vp1.transform.position;
        Vector3 x2 = vp2.transform.position;
        Vector3 d1 = x2 - x1;
        float d2 = d1.magnitude;
        float d3 = (d2 - constraintLength) / d2;
        vp1.transform.position = x1 + 0.5f * d1 * d3;
        vp2.transform.position = x2 - 0.5f * d1 * d3;
    }
}
