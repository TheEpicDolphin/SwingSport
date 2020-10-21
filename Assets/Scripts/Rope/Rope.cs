using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    /* Any objects that are attached to this rope */
    LinkedList<RopeAttachment> ropeAttachments = new LinkedList<RopeAttachment>();

    /* The verlet particles that make up the rope structure */
    LinkedList<VerletParticle> verletParticles = new LinkedList<VerletParticle>();

    /* The preferred spacing between verlet particles */
    const float verletParticleSpacing = 1.0f;

    /* Small float value */
    const float delta = 1e-4f;

    /* The length of the rope when not stretched */
    public float RestLength
    {
        get
        {
            return verletParticles.Last.Value.restPosition;
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
        /* Check if verlet particles should be spaced out more evenly */
        if (Mathf.Abs(RestLength - (verletParticleSpacing * verletParticles.Count)) > 5.0f)
        {
            RecalculateVerletParticles();
        }

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
        if (currentVPNode.Value.restPosition < currentRANode.Value.restPosition)
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
            if (currentRANode == null || currentVPNode.Value.restPosition < currentRANode.Value.restPosition)
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

    public void SortUp(RopeAttachment ra)
    {
        LinkedListNode<RopeAttachment> ropeAttachmentNode = ropeAttachments.Find(ra);
        LinkedListNode<RopeAttachment> nextRANode = ropeAttachmentNode.Next;
        ropeAttachments.Remove(ropeAttachmentNode);
        while (nextRANode != null && ra.restPosition > nextRANode.Value.restPosition)
        {
            nextRANode = nextRANode.Next;
        }
        ropeAttachments.AddBefore(nextRANode, ra);
    }

    public void SortDown(RopeAttachment ra)
    {
        LinkedListNode<RopeAttachment> ropeAttachmentNode = ropeAttachments.Find(ra);
        LinkedListNode<RopeAttachment> previousRANode = ropeAttachmentNode.Previous;
        ropeAttachments.Remove(ropeAttachmentNode);
        while (previousRANode != null && ra.restPosition < previousRANode.Value.restPosition)
        {
            previousRANode = previousRANode.Previous;
        }
        ropeAttachments.AddAfter(previousRANode, ra);
    }

    public void InsertRope(float insertionPosition, float amount)
    {
        LinkedListNode<VerletParticle> nextVP = FindClosestRopeNodeAfter(verletParticles, insertionPosition);
        LinkedListNode<RopeAttachment> nextRA = FindClosestRopeNodeAfter(ropeAttachments, insertionPosition);
        OffsetRestPositionsBeginningFrom(nextVP, amount);
        OffsetRestPositionsBeginningFrom(nextRA, amount);

        LinkedListNode<VerletParticle> previousVP = nextVP.Previous;
        Vector3 startPos = Vector3.Lerp(previousVP.Value.transform.position, 
                                        nextVP.Value.transform.position, 
                                        (insertionPosition - previousVP.Value.restPosition) / (nextVP.Value.restPosition - previousVP.Value.restPosition));
        Vector3 endPos = nextVP.Value.transform.position;
        float fillLength = nextVP.Value.restPosition - insertionPosition;
        float currentPosition = nextVP.Value.restPosition - verletParticleSpacing;
        LinkedListNode<VerletParticle> ceiling = nextVP;
        while (currentPosition > insertionPosition)
        {
            GameObject vpGO = new GameObject();
            float t = (currentPosition - insertionPosition) / fillLength;
            vpGO.transform.position = Vector3.Lerp(startPos, endPos, t); ;
            VerletParticle vp = vpGO.AddComponent<VerletParticle>();
            ceiling = verletParticles.AddBefore(ceiling, vp);
            currentPosition -= verletParticleSpacing;
        }
    }

    public void RemoveRope(float removalPosition, float amount)
    {        
        LinkedListNode<VerletParticle> nextVP = FindClosestRopeNodeAfter(verletParticles, removalPosition);
        LinkedListNode<RopeAttachment> nextRA = FindClosestRopeNodeAfter(ropeAttachments, removalPosition);

        OffsetRestPositionsBeginningFrom(nextVP, amount);
        LinkedListNode<VerletParticle> currentVPNode = nextVP;
        while (currentVPNode != null && nextVP.Value.restPosition > currentVPNode.Value.restPosition)
        {
            VerletParticle vp = currentVPNode.Value;
            verletParticles.Remove(currentVPNode);
            Destroy(vp);
            currentVPNode = currentVPNode.Next;
        }

        OffsetRestPositionsBeginningFrom(nextRA, amount);
        LinkedListNode<RopeAttachment> currentRANode = nextRA;
        while (currentRANode != null && removalPosition > currentRANode.Value.restPosition)
        {
            RopeAttachment ra = currentRANode.Value;
            Destroy(ra);
            currentRANode = currentRANode.Next;
        }
    }

    private void OffsetRestPositionsBeginningFrom<T>(LinkedListNode<T> ropeNode, float offset) where T : RopeNode
    {
        LinkedListNode<T> currentNode = ropeNode;
        while (currentNode != null)
        {
            currentNode.Value.restPosition += offset;
            currentNode = currentNode.Next;
        }
    }

    private LinkedListNode<T> FindClosestRopeNodeAfter<T>(LinkedList<T> ropeNodes, float position) where T : RopeNode
    {
        LinkedListNode<T> currentNode = ropeNodes.First;
        while (currentNode != null)
        {
            if (currentNode.Value.restPosition > position)
            {
                return currentNode;
            }
            currentNode = currentNode.Next;
        }
        return null;
    }

    private void RecalculateVerletParticles()
    {
        // TODO: Code this later
    }

    public void CreateRope(Vector3 startPos, Vector3 endPos)
    {
        float length = Vector3.Distance(startPos, endPos);
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

    public void Detach(RopeAttachment ra)
    {
        ropeAttachments.Remove(ra);
    }

    public void Attach(RopeAttachment ra)
    {
        LinkedListNode<RopeAttachment> currentNode = ropeAttachments.First;
        while (currentNode != null)
        {
            if (ra.restPosition < currentNode.Value.restPosition)
            {
                ropeAttachments.AddBefore(currentNode, ra);
                return;
            }
            currentNode = currentNode.Next;
        }
        ropeAttachments.AddLast(ra);
    }
}
