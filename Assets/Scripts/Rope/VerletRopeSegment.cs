using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerletRopeSegment
{
    RopeAttachment start;
    RopeAttachment end;

    public List<VerletRopeNode> ropeNodes = new List<VerletRopeNode>();
    private List<Vector3> ropeNodePositions;

    public VerletRopeSegment(RopeAttachment start, RopeAttachment end)
    {
        this.start = start;
        this.end = end;
    }

    void Run()
    {
        if(start.ropeLocation > end.ropeLocation)
        {
            RopeAttachment temp = start;
            end = temp;
        }

        Simulate();

        /* Higher iteration results in stiffer ropes and stable simulation */
        for (int i = 0; i < 80; i++)
        {
            ApplyConstraints();

            /* Playing around with adjusting collisions at intervals - still stable when iterations are skipped */
            if (i % 2 == 1)
            {
                //AdjustCollisions();
            }
        }
    }

    void InterpolateNodes()
    {

    }

}
