using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerletParticle : RopeNode
{
    public Vector3 previousPosition;

    public override void Simulate()
    {
        // derive the velocity from previous frame
        Vector3 velocity = transform.position - previousPosition;
        previousPosition = transform.position;

        // calculate new position
        Vector3 newPos = transform.position + velocity;
        newPos += Physics.gravity * Time.fixedDeltaTime * Time.fixedDeltaTime;
        transform.position = newPos;
    }
}
