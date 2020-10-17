using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    LinkedList<RopeNode> ropeNodes = new LinkedList<RopeNode>();

    float maxSpaceBetweenNodes = 2.0f;
    float minSpaceBetweenNodes = 0.8f;

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
        /* Sort rope nodes just in case they switched order since the last fixed update */
        Utils.InsertionSort(ropeNodes);

        //TODO: perform raycasting along rope to allow wrapping around objects

        /* Simulate rope nodes */
        foreach (RopeNode node in ropeNodes)
        {
            node.Simulate();
        }

        /* Apply constraints */
        LinkedListNode<RopeNode> currentNode = ropeNodes.First;
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


    }

    public void AttachToRope(Rigidbody attachedRb, Transform attachedTransform)
    {
        RopeAttachment newRopeAttachment = new RopeAttachment(attachedRb, attachedTransform);
    }

    private void InsertVerletParticle()
    {

    }

    private void RemoveVerletParticle()
    {

    }
}
