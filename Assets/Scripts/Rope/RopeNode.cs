using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RopeNode : MonoBehaviour
{
    /* Location on rope when unstretched */

    public float ropeLocation;

    public abstract void Simulate();

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

    public static void ApplyConstraint(RopeAttachment ra1, RopeAttachment ra2)
    {
        //Don't apply constraint on rope attachments. We apply forces instead later
    }

    public static void ApplyConstraint(RopeAttachment ra, VerletParticle vp)
    {
        float constraintLength = vp.ropeLocation - ra.ropeLocation;
        Vector3 d1 = vp.transform.position - ra.transform.position;
        float d2 = d1.magnitude;
        float d3 = (d2 - constraintLength) / d2;
        vp.transform.position += -1.0f * d1 * d3;
    }

    public static void ApplyConstraint(VerletParticle vp, RopeAttachment ra)
    {
        float constraintLength = ra.ropeLocation - vp.ropeLocation;
        Vector3 d1 = ra.transform.position - vp.transform.position;
        float d2 = d1.magnitude;
        float d3 = (d2 - constraintLength) / d2;
        vp.transform.position += 1.0f * d1 * d3;
    }

    
}
