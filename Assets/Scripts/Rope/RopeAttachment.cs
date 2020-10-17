using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeAttachment
{
    /* Location on rope when unstretched */
    public float ropeLocation;

    public Rigidbody rb;

    public Transform transform;

    public RopeAttachment(float ropeLocation, Rigidbody rb, Transform transform)
    {
        this.ropeLocation = ropeLocation;
        this.rb = rb;
        this.transform = transform;
    }

}
