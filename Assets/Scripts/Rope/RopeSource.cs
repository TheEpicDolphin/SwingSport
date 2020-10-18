using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeSource
{
    Rope rope;
    RopeAttachment ra;

    private void FixedUpdate()
    {
        rope.SimulateWithSource(ra);
    }
}
