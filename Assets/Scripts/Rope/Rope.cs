using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    LinkedList<RopeAttachment> ropeAttachments = new LinkedList<RopeAttachment>();

    /* The verlet particles that make up the rope.
       These will never be out of order. We're just pushing to and popping from this */
    LinkedList<VerletParticle> verletParticles = new LinkedList<VerletParticle>();
    const float verletParticleSpacing = 1.0f;

    float length;

    private void Awake()
    {
        
    }

    private void Update()
    {
        Draw();
    }

    private void Draw()
    {
        
    }

    private void FixedUpdate()
    {
        /* Sort rope attachments just in case they switched order since the last fixed update */
        Utils.InsertionSort(ropeAttachments);

        //TODO: perform raycasting along rope to allow wrapping around objects

        /* Simulate verlet particles */
        foreach (VerletParticle vp in verletParticles)
        {
            vp.Simulate();
        }

        /* Apply constraints */
        LinkedListNode<ropeAttachments> currentNode = ropeNodes.First;
        while (currentNode != null && currentNode.Next != null)
        {
            RopeNode.ApplyConstraint(currentNode.Value, currentNode.Next.Value);
            currentNode = currentNode.Next;
        }

        LinkedList<RopeAttachment> ropeAttachments = new LinkedList<RopeAttachment>();


        while (currentNode != null && currentNode.Next != null)
        {
            RopeNode.ApplyConstraint(currentNode.Value, currentNode.Next.Value);
            currentNode = currentNode.Next;
        }

        /* Add/remove verlet particles if there is too little/much space between nodes */
        int numVerletParticles = verletParticles.Count;
        int d = Mathf.FloorToInt(length / verletParticleSpacing) - numVerletParticles;
        for(int i = 0; i < Mathf.Abs(d); i++)
        {
            if (d >= 0)
            {
                verletParticles.AddLast();
            }
            else
            {
                verletParticles.RemoveLast();
            }
        }        
    }

    public void AttachToRope(Rigidbody attachedRb, Transform attachedTransform)
    {
        RopeAttachment newRopeAttachment = new RopeAttachment(attachedRb, attachedTransform);
    }
}
