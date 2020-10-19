using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeSource
{
    Rope rope;
    private float ropeLocation;
    public float RopeLocation
    {
        get
        {
            return ropeLocation;
        }
        set
        {
            ropeLocation = Mathf.Clamp(value, 0.0f, rope.RestLength);
        }
    }

    public void InsertRope(float amount)
    {
        rope.InsertRope(ropeLocation, amount);
    }

    public void RemoveRope(float amount)
    {
        rope.RemoveRope(ropeLocation, amount);
    }
}
