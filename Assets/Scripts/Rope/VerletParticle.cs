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

    public override void ApplyConstraint(VerletParticle vp)
    {
        float constraintLength = vp.restPosition - restPosition;
        Vector3 x1 = transform.position;
        Vector3 x2 = vp.transform.position;
        Vector3 d1 = x2 - x1;
        float d2 = d1.magnitude;
        float d3 = (d2 - constraintLength) / d2;
        transform.position = x1 + 0.5f * d1 * d3;
        vp.transform.position = x2 - 0.5f * d1 * d3;
    }

    public override void ApplyConstraint(RopeAttachment ra)
    {
        float constraintLength = ra.restPosition - restPosition;
        Vector3 d1 = ra.transform.position - transform.position;
        float d2 = d1.magnitude;
        float d3 = (d2 - constraintLength) / d2;
        transform.position += 1.0f * d1 * d3;
    }
}
