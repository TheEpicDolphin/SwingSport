using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RopeNode : MonoBehaviour
{
    /* Location on rope when unstretched */

    public float ropeLocation;

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
