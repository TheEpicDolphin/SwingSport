﻿using System.Collections;
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

        /* Apply vp--vp, vp--ra, and ra--vp constraints */
        LinkedListNode<VerletParticle> currentVPNode = verletParticles.First;
        LinkedListNode<RopeAttachment> currentRANode = ropeAttachments.First;
        RopeNode lastRopeNode;
        if (currentVPNode.Value.ropeLocation < currentRANode.Value.ropeLocation)
        {
            lastRopeNode = currentVPNode.Value;
            currentVPNode = currentVPNode.Next;
        }
        else
        {
            lastRopeNode = currentRANode.Value;
            currentRANode = currentRANode.Next;
        }
        while (currentVPNode != null)
        {
            if (currentRANode == null || currentVPNode.Value.ropeLocation < currentRANode.Value.ropeLocation)
            {
                lastRopeNode.ApplyConstraint(currentVPNode.Value);
                lastRopeNode = currentVPNode.Value;
                currentVPNode = currentVPNode.Next;
            }
            else
            {
                lastRopeNode.ApplyConstraint(currentRANode.Value);
                lastRopeNode = currentRANode.Value;
                currentRANode = currentRANode.Next;
            }
        }

        /* Apply ra--ra constraints */
        LinkedListNode<RopeAttachment> currentNode = ropeAttachments.First;
        while (currentNode != null && currentNode.Next != null)
        {
            RopeAttachment.ApplyTension(currentNode.Value, currentNode.Next.Value);
            currentNode = currentNode.Next;
        }

        /* Add/remove verlet particles if there is too little/much space between nodes */
        int numVerletParticles = verletParticles.Count;
        int d = Mathf.FloorToInt(length / verletParticleSpacing) - numVerletParticles;
        for(int i = 0; i < Mathf.Abs(d); i++)
        {
            if (d >= 0)
            {
                GameObject vpGOToAdd = new GameObject();
                vpGOToAdd.transform.position = verletParticles.Last.Value.transform.position;
                vpGOToAdd.transform.rotation = Quaternion.identity;
                VerletParticle vpToAdd = vpGOToAdd.AddComponent<VerletParticle>();
                verletParticles.AddLast(vpToAdd);
            }
            else
            {
                VerletParticle vpToRemove = verletParticles.Last.Value;
                verletParticles.RemoveLast();
                Destroy(vpToRemove.gameObject);
            }
        }        
    }

    public void CreateRope(RopeAttachment ra1, RopeAttachment ra2)
    {
        CreateVerletParticles(ra1.attachmentTransform.position, ra2.attachmentTransform.position);
        ropeAttachments.AddLast(ra1);
        ropeAttachments.AddLast(ra2);
    }

    public void CreateVerletParticles(Vector3 startPos, Vector3 endPos)
    {
        length = Vector3.Distance(startPos, endPos);
        float t = 0.0f;
        int n = Mathf.FloorToInt(length / verletParticleSpacing);
        for (int i = 0; i < n; i++)
        {
            t += i * verletParticleSpacing;
            GameObject vpGO = new GameObject();
            vpGO.transform.position = Vector3.Lerp(startPos, endPos, t); ;
            vpGO.transform.rotation = Quaternion.identity;
            VerletParticle vp = vpGO.AddComponent<VerletParticle>();
            verletParticles.AddLast(vp);
        }
        GameObject endVPGO = new GameObject();
        endVPGO.transform.position = endPos;
        endVPGO.transform.rotation = Quaternion.identity;
        VerletParticle endVP = endVPGO.AddComponent<VerletParticle>();
        verletParticles.AddLast(endVP);
    }

    public void AttachToRope(Rigidbody attachedRb, Transform attachedTransform)
    {
        RopeAttachment newRopeAttachment = new RopeAttachment(attachedRb, attachedTransform);
    }
}
