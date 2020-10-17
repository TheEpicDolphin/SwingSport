using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    LinkedList<RopeNode> ropeNodes = new LinkedList<RopeNode>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void FixedUpdate()
    {
        Utils.InsertionSort(ropeAttachments);


        for(int i = 0; i < ropeAttachments.Count - 1; i++)
        {

        }
    }

    public void AttachToRope(Rigidbody attachedRb, Transform attachedTransform)
    {
        RopeAttachment newRopeAttachment = new RopeAttachment(attachedRb, attachedTransform);

    }

    
}
