using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RopeNode : MonoBehaviour
{
    /* Smaller restPosition is closer to start of rope */
    public float restPosition;

    public Rope rope;

    public abstract void ApplyConstraint(RopeAttachment ra);

    public abstract void ApplyConstraint(VerletParticle vp);

    public abstract Vector3 AttachmentPoint();
}
