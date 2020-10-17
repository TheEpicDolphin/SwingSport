using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeAttachment : RopeNode
{
    public Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void Simulate()
    {
        //Do nothing
    }

}
