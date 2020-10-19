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

    /* Rope rest lenth does not officially change, what changes is the amount visible */
    private float restLength;
    public float RestLength
    {
        get
        {
            return restLength;
        }
    }

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
        RopeNode previousRopeNode;
        if (currentVPNode.Value.ropeLocation < currentRANode.Value.ropeLocation)
        {
            previousRopeNode = currentVPNode.Value;
            currentVPNode = currentVPNode.Next;
        }
        else
        {
            previousRopeNode = currentRANode.Value;
            currentRANode = currentRANode.Next;
        }
        while (currentVPNode != null)
        {
            if (currentRANode == null || currentVPNode.Value.ropeLocation < currentRANode.Value.ropeLocation)
            {
                previousRopeNode.ApplyConstraint(currentVPNode.Value);
                previousRopeNode = currentVPNode.Value;
                currentVPNode = currentVPNode.Next;
            }
            else
            {
                previousRopeNode.ApplyConstraint(currentRANode.Value);
                previousRopeNode = currentRANode.Value;
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
    }

    public void SimulateWithSource(RopeAttachment source)
    {
        /* Sort rope attachments just in case they switched order since the last fixed update */
        Utils.InsertionSort(ropeAttachments);

        /* Remove nodes attached to rope that the ropeSource has passed */
        while (source.ropeLocation > ropeAttachments.First.Value.ropeLocation)
        {
            RopeAttachment ra = ropeAttachments.First.Value;
            ropeAttachments.RemoveFirst();
            Destroy(ra);
        }
        while (source.ropeLocation > verletParticles.First.Value.ropeLocation)
        {
            VerletParticle vp = verletParticles.First.Value;
            verletParticles.RemoveFirst();
            Destroy(vp);
        }

        /* Create verlet nodes  */

        //TODO: perform raycasting along rope to allow wrapping around objects

        /* Simulate verlet particles */
        foreach (VerletParticle vp in verletParticles)
        {
            vp.Simulate();
        }

        /* Apply vp--vp, vp--ra, and ra--vp constraints */
        LinkedListNode<VerletParticle> currentVPNode = verletParticles.First;
        LinkedListNode<RopeAttachment> currentRANode = ropeAttachments.First;
        RopeNode previousRopeNode = source;
        while (currentVPNode != null)
        {
            if (currentRANode == null || currentVPNode.Value.ropeLocation < currentRANode.Value.ropeLocation)
            {
                previousRopeNode.ApplyConstraint(currentVPNode.Value);
                previousRopeNode = currentVPNode.Value;
                currentVPNode = currentVPNode.Next;
            }
            else
            {
                previousRopeNode.ApplyConstraint(currentRANode.Value);
                previousRopeNode = currentRANode.Value;
                currentRANode = currentRANode.Next;
            }
        }

        /* Apply ra--ra constraints */
        LinkedListNode<RopeAttachment> currentNode = ropeAttachments.First;
        RopeAttachment.ApplyTension(source, currentNode.Value);
        while (currentNode != null && currentNode.Next != null)
        {
            RopeAttachment.ApplyTension(currentNode.Value, currentNode.Next.Value);
            currentNode = currentNode.Next;
        }
    }

    public void InsertRope(float ropeLocation, float amount)
    {
        LinkedListNode<VerletParticle> closestVerletParticleAfterLocation = FindClosestRopeNodeAfter(ropeLocation);
        LinkedListNode<RopeAttachment> closestRopeAttachmentAfterLocation = FindClosestRopeNodeAfter(ropeLocation);
        OffsetRopeNodeLocationsBeginningFrom(closestVerletParticleAfterLocation.Next, amount);
        OffsetRopeNodeLocationsBeginningFrom(closestRopeAttachmentAfterLocation.Next, amount);

        //TODO: Finish this
        Vector3 startPos = closestVerletParticleAfterLocation.Next.Value.ropeLocation;
        float fillLength = closestVerletParticleAfterLocation.Next.Value.ropeLocation - 
            closestVerletParticleAfterLocation.Value.ropeLocation;
        float t = 0.0f;
        int n = Mathf.FloorToInt(length / verletParticleSpacing);
        for (int i = 0; i < n; i++)
        {
            t += i * verletParticleSpacing;
            GameObject vpGO = new GameObject();
            vpGO.transform.position = Vector3.Lerp(startPos, endPos, t); ;
            VerletParticle vp = vpGO.AddComponent<VerletParticle>();
            verletParticles.AddBefore(, vp);
        }
    }

    public void RemoveRope(float ropeLocation, float amount)
    {        
        LinkedListNode<VerletParticle> closestVerletParticleAfterLocation = FindClosestRopeNodeAfter(ropeLocation);
        LinkedListNode<RopeAttachment> closestRopeAttachmentAfterLocation = FindClosestRopeNodeAfter(ropeLocation);
        OffsetRopeNodeLocationsBeginningFrom(closestVerletParticleAfterLocation.Next, amount);
        OffsetRopeNodeLocationsBeginningFrom(closestRopeAttachmentAfterLocation.Next, amount);
        LinkedListNode<VerletParticle> currentVPNode = closestVerletParticleAfterLocation.Next;
        while (currentVPNode != null && 
            closestVerletParticleAfterLocation.Value.ropeLocation > currentVPNode.Value.ropeLocation)
        {
            VerletParticle vp = currentVPNode.Value;
            verletParticles.Remove(currentVPNode);
            Destroy(vp);
            currentVPNode = currentVPNode.Next;
        }
        LinkedListNode<RopeAttachment> currentRANode = closestRopeAttachmentAfterLocation.Next;
        while (currentRANode != null && ropeLocation > currentRANode.Value.ropeLocation)
        {
            RopeAttachment ra = currentRANode.Value;
            ropeAttachments.Remove(currentRANode);
            Destroy(ra);
            currentRANode = currentRANode.Next;
        }
    }

    private void OffsetRopeNodeLocationsBeginningFrom<T>(LinkedListNode<T> ropeNode, float offset) where T : RopeNode
    {
        LinkedListNode<T> currentNode = ropeNode;
        while (currentNode != null)
        {
            currentNode.Value.ropeLocation += offset;
            currentNode = currentNode.Next;
        }
    }

    private LinkedListNode<RopeNode> FindClosestRopeNodeAfter(float ropeLocation)
    {
        
    }



    public void CreateRope(Vector3 startPos, Vector3 endPos, float sourceBuffer)
    {
        length = Vector3.Distance(startPos, endPos);
        float t = 0.0f;
        int n = Mathf.FloorToInt(length / verletParticleSpacing);
        for (int i = 0; i < n; i++)
        {
            t += i * verletParticleSpacing;
            GameObject vpGO = new GameObject();
            vpGO.transform.position = Vector3.Lerp(startPos, endPos, t); ;
            VerletParticle vp = vpGO.AddComponent<VerletParticle>();
            verletParticles.AddLast(vp);
        }
        GameObject endVPGO = new GameObject();
        endVPGO.transform.position = endPos;
        VerletParticle endVP = endVPGO.AddComponent<VerletParticle>();
        verletParticles.AddLast(endVP);
    }

    public void Attach(Rigidbody attachedRb, Transform attachedTransform)
    {
        RopeAttachment newRopeAttachment = new RopeAttachment(attachedRb, attachedTransform);
    }
}
