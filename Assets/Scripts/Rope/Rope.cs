using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    /* For drawing the rope */
    private LineRenderer ropeRenderer;

    /* This list will be used to draw the rope */
    private List<Vector3> ropeNodePositions;

    /* Any objects that are attached to this rope */
    private LinkedList<RopeAttachment> ropeAttachments = new LinkedList<RopeAttachment>();

    /* The verlet particles that make up the rope structure */
    private LinkedList<VerletParticle> verletParticles = new LinkedList<VerletParticle>();

    /* The preferred spacing between verlet particles */
    private const float verletParticleSpacing = 1.0f;

    /* Small float value */
    private const float delta = 1e-4f;

    /* The length of the rope when not stretched */
    public float RestLength
    {
        get
        {
            if (verletParticles.Last != null)
            {
                return verletParticles.Last.Value.restPosition;
            }
            else
            {
                return 0.0f;
            }
        }
    }

    private void Awake()
    {
        ropeRenderer = gameObject.AddComponent<LineRenderer>();
        ropeRenderer.widthMultiplier = 0.05f;
        ropeRenderer.material = new Material(Shader.Find("Unlit/Color"));
        ropeRenderer.material.color = Color.red;
    }

    private void Update()
    {
        Draw();
    }

    private void Draw()
    {
        ropeNodePositions = new List<Vector3>();
        LinkedListNode<VerletParticle> currentVPNode = verletParticles.First;
        LinkedListNode<RopeAttachment> currentRANode = ropeAttachments.First;
        while (currentVPNode != null)
        {
            if (currentRANode == null || 
                currentVPNode.Value.restPosition < currentRANode.Value.restPosition)
            {
                ropeNodePositions.Add(currentVPNode.Value.transform.position);
                currentVPNode = currentVPNode.Next;
            }
            else
            {
                ropeNodePositions.Add(currentRANode.Value.transform.position);
                currentRANode = currentRANode.Next;
            }
        }
        ropeRenderer.positionCount = ropeNodePositions.Count;
        ropeRenderer.SetPositions(ropeNodePositions.ToArray());
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

        for(int i = 0; i < 80; i++)
        {
            ApplyConstraints();
        }

        /* Apply ra--ra forces */
        LinkedListNode<RopeAttachment> currentNode = ropeAttachments.First;
        while (currentNode != null && currentNode.Next != null)
        {
            RopeAttachment.ApplyTension(currentNode.Value, currentNode.Next.Value);
            currentNode = currentNode.Next;
        }      
    }

    private void ApplyConstraints()
    {
        /* Apply vp--vp, vp--ra, and ra--vp constraints */
        LinkedListNode<VerletParticle> currentVPNode = verletParticles.First;
        LinkedListNode<RopeAttachment> currentRANode = ropeAttachments.First;
        RopeNode previousRopeNode = null;
        while (currentVPNode != null)
        {
            if (currentRANode == null || currentVPNode.Value.restPosition < currentRANode.Value.restPosition)
            {
                if (previousRopeNode)
                {
                    previousRopeNode.ApplyConstraint(currentVPNode.Value);
                }
                previousRopeNode = currentVPNode.Value;
                currentVPNode = currentVPNode.Next;
            }
            else
            {
                if (previousRopeNode)
                {
                    previousRopeNode.ApplyConstraint(currentRANode.Value);
                }
                previousRopeNode = currentRANode.Value;
                currentRANode = currentRANode.Next;
            }
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
        insertionPosition = ClampPositionToRopeExtents(insertionPosition);
        LinkedListNode<VerletParticle> nextVP = FindClosestRopeNodeAfter(verletParticles, insertionPosition);
        LinkedListNode<RopeAttachment> nextRA = FindClosestRopeNodeAfter(ropeAttachments, insertionPosition);
        if (nextVP == verletParticles.First || nextVP == null)
        {
            //This should never happen
            Debug.LogError("Insertion rope position is out of bounds!");
            return;
        }
        LinkedListNode<VerletParticle> previousVP = nextVP.Previous;
        Vector3 startPos = Vector3.Lerp(previousVP.Value.transform.position,
                                        nextVP.Value.transform.position,
                                        (insertionPosition - previousVP.Value.restPosition) / (nextVP.Value.restPosition - previousVP.Value.restPosition));

        OffsetRestPositionsBeginningFrom(nextVP, amount);
        OffsetRestPositionsBeginningFrom(nextRA, amount);

        Vector3 endPos = nextVP.Value.transform.position;
        float fillLength = nextVP.Value.restPosition - insertionPosition;
        float currentPosition = nextVP.Value.restPosition - verletParticleSpacing;
        LinkedListNode<VerletParticle> ceiling = nextVP;
        while (currentPosition > insertionPosition)
        {
            GameObject vpGO = new GameObject();
            float t = (currentPosition - insertionPosition) / fillLength;
            VerletParticle vp = vpGO.AddComponent<VerletParticle>();
            vp.transform.position = Vector3.Lerp(startPos, endPos, t);
            vp.previousPosition = vp.transform.position;
            ceiling = verletParticles.AddBefore(ceiling, vp);
            currentPosition -= verletParticleSpacing;
        }
    }

    public void RemoveRope(float removalPosition, float amount)
    {
        removalPosition = ClampPositionToRopeExtents(removalPosition);
        LinkedListNode<VerletParticle> nextVP = FindClosestRopeNodeAfter(verletParticles, removalPosition);
        LinkedListNode<RopeAttachment> nextRA = FindClosestRopeNodeAfter(ropeAttachments, removalPosition);
        if (nextVP == verletParticles.First || nextVP == null)
        {
            //This should never happen
            Debug.LogError("Insertion rope position is out of bounds!");
            return;
        }

        OffsetRestPositionsBeginningFrom(nextRA, amount);
        LinkedListNode<RopeAttachment> currentRANode = nextRA;
        while (currentRANode != null && removalPosition > currentRANode.Value.restPosition)
        {
            RopeAttachment ra = currentRANode.Value;
            Destroy(ra);
            currentRANode = currentRANode.Next;
        }

        OffsetRestPositionsBeginningFrom(nextVP, amount);
        LinkedListNode<VerletParticle> currentVPNode = nextVP;
        while (removalPosition > currentVPNode.Value.restPosition)
        {
            if (currentVPNode.Next == null)
            {
                currentVPNode.Value.restPosition = removalPosition + delta;
                break;
            }
            VerletParticle vp = currentVPNode.Value;
            verletParticles.Remove(currentVPNode);
            Destroy(vp);
            currentVPNode = currentVPNode.Next;
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

    public static Rope CreateTautRope(Vector3 startPos, Vector3 endPos)
    {
        GameObject ropeGO = new GameObject();
        Rope rope = ropeGO.AddComponent<Rope>();
        float length = Vector3.Distance(startPos, endPos);
        float x = 0.0f;
        int n = Mathf.FloorToInt(length / verletParticleSpacing);

        GameObject startVPGO = new GameObject();
        VerletParticle startVP = startVPGO.AddComponent<VerletParticle>();
        startVP.transform.position = startPos;
        startVP.previousPosition = startVP.transform.position;
        startVP.restPosition = x;
        rope.verletParticles.AddLast(startVP);
        for (int i = 1; i <= n; i++)
        {
            x += verletParticleSpacing;
            GameObject vpGO = new GameObject();
            VerletParticle vp = vpGO.AddComponent<VerletParticle>();
            vp.transform.position = Vector3.Lerp(startPos, endPos, x / length);
            vp.previousPosition = vp.transform.position;
            vp.restPosition = x;
            rope.verletParticles.AddLast(vp);
        }
        GameObject endVPGO = new GameObject();
        VerletParticle endVP = endVPGO.AddComponent<VerletParticle>();
        endVP.transform.position = endPos;
        endVP.previousPosition = endVP.transform.position;
        endVP.restPosition = length;
        rope.verletParticles.AddLast(endVP);
        return rope;
    }

    public float ClampPositionToRopeExtents(float position)
    {
        return Mathf.Clamp(position, delta, RestLength - delta);
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
