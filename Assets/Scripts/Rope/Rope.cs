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
    const float delta = 1e-4f;

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
        /* Now sort them because their rope locations have changed */
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

    public void QueueMovement(RopeAttachment ra, float dt)
    {
        
    }

    public void InsertRope(float ropeLocation, float amount)
    {
        LinkedListNode<VerletParticle> nextVP = FindClosestRopeNodeAfter(verletParticles, ropeLocation);
        LinkedListNode<RopeAttachment> nextRA = FindClosestRopeNodeAfter(ropeAttachments, ropeLocation);
        OffsetRopeNodeLocationsBeginningFrom(nextVP, amount);
        OffsetRopeNodeLocationsBeginningFrom(nextRA, amount);

        LinkedListNode<VerletParticle> previousVP = nextVP.Previous;
        Vector3 startPos = Vector3.Lerp(previousVP.Value.transform.position, nextVP.Value.transform.position, (ropeLocation - previousVP.Value.ropeLocation) / (nextVP.Value.ropeLocation - previousVP.Value.ropeLocation));
        Vector3 endPos = nextVP.Value.transform.position;
        float fillLength = nextVP.Value.ropeLocation - ropeLocation;
        float t = nextVP.Value.ropeLocation - verletParticleSpacing;
        LinkedListNode<VerletParticle> ceiling = nextVP;
        while (t > ropeLocation)
        {
            GameObject vpGO = new GameObject();
            vpGO.transform.position = Vector3.Lerp(startPos, endPos, (t - ropeLocation) / fillLength); ;
            VerletParticle vp = vpGO.AddComponent<VerletParticle>();
            ceiling = verletParticles.AddBefore(ceiling, vp);
            t -= verletParticleSpacing;
        }
    }

    public void RemoveRope(float ropeLocation, float amount)
    {        
        LinkedListNode<VerletParticle> nextVP = FindClosestRopeNodeAfter(verletParticles, ropeLocation);
        LinkedListNode<RopeAttachment> nextRA = FindClosestRopeNodeAfter(ropeAttachments, ropeLocation);

        OffsetRopeNodeLocationsBeginningFrom(nextVP, amount);
        LinkedListNode<VerletParticle> currentVPNode = nextVP;
        while (currentVPNode != null &&
            nextVP.Value.ropeLocation > currentVPNode.Value.ropeLocation)
        {
            VerletParticle vp = currentVPNode.Value;
            verletParticles.Remove(currentVPNode);
            Destroy(vp);
            currentVPNode = currentVPNode.Next;
        }

        OffsetRopeNodeLocationsBeginningFrom(nextRA, amount);
        LinkedListNode<RopeAttachment> currentRANode = nextRA.Next;
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

    private LinkedListNode<T> FindClosestRopeNodeAfter<T>(LinkedList<T> ropeNodes, float ropeLocation) where T : RopeNode
    {
        LinkedListNode<T> currentNode = ropeNodes.First;
        while (currentNode != null)
        {
            if (currentNode.Value.ropeLocation > ropeLocation)
            {
                return currentNode;
            }
            currentNode = currentNode.Next;
        }
        return null;
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
