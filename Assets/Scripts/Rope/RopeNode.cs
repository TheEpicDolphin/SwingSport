using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RopeNode : MonoBehaviour
{
    /* Smaller t is closer to start of rope */
    public float ropeLocation;

    public abstract void ApplyConstraint(RopeAttachment ra);

    public abstract void ApplyConstraint(VerletParticle vp);
}
