﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GeometryUtils;

public class Rope : MonoBehaviour
{
    /* For drawing the rope */
    private LineRenderer ropeRenderer;

    /* This list will be used to draw the rope */
    private List<Vector3> ropeNodePositions;

    /* Any objects that are attached to this rope */
    //private LinkedList<RopeAttachment> ropeAttachments = new LinkedList<RopeAttachment>();
    private LinkedList<IRopeAttachment> ropeAttachments = new LinkedList<IRopeAttachment>();

    /* The verlet particles that make up the rope structure */
    private LinkedList<VerletParticle> verletParticles = new LinkedList<VerletParticle>();

    /* The preferred spacing between verlet particles */
    private const float verletParticleSpacing = 1.0f;

    /* Small float value */
    private const float delta = 1e-4f;

    private Dictionary<IRopeNode, float> restPositionMap;

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
        IEnumerator<RopeNode> ropeNodes = GetAllRopeNodes();
        while (ropeNodes.MoveNext())
        {
            ropeNodePositions.Add(ropeNodes.Current.AttachmentPoint());
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
        LinkedListNode<IRopeAttachment> currentNode = ropeAttachments.First;
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
        amount = Mathf.Abs(amount);
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
            vp.restPosition = currentPosition;
            ceiling = verletParticles.AddBefore(ceiling, vp);
            currentPosition -= verletParticleSpacing;
        }
    }

    public void RemoveRope(float removalPosition, float amount)
    {
        amount = -Mathf.Abs(amount);
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
            LinkedListNode<VerletParticle> temp = currentVPNode.Next;
            VerletParticle vp = currentVPNode.Value;
            verletParticles.Remove(currentVPNode);
            Destroy(vp.gameObject);
            currentVPNode = temp;
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

    public static Rope CreateInterpolatedRope(Vector3[] points)
    {
        List<Vector3> interpolatedPoints = new List<Vector3>();
        interpolatedPoints.Add(points[0]);
        int idx = 1;
        Vector3 rayOrigin = interpolatedPoints[interpolatedPoints.Count - 1];
        while (idx < points.Length)
        {
            if (Vector3.Distance(interpolatedPoints[interpolatedPoints.Count - 1], points[idx])
                >= verletParticleSpacing)
            {
                Vector3 intersection;
                if (Geometry.RaySphereExitIntersection(
                    new Ray(rayOrigin, (points[idx] - points[idx - 1]).normalized),
                    interpolatedPoints[interpolatedPoints.Count - 1],
                    verletParticleSpacing,
                    out intersection))
                {
                    interpolatedPoints.Add(intersection);
                    rayOrigin = interpolatedPoints[interpolatedPoints.Count - 1];
                    continue;
                }
            }
            rayOrigin = points[idx];
            idx += 1;
        }
        if(interpolatedPoints[interpolatedPoints.Count - 1] != points[points.Length - 1])
        {
            interpolatedPoints.Add(points[points.Length - 1]);
        }

        GameObject ropeGO = new GameObject();
        Rope rope = ropeGO.AddComponent<Rope>();
        float x = 0.0f;
        foreach(Vector3 interpolatedPoint in interpolatedPoints)
        {
            GameObject vpGO = new GameObject();
            VerletParticle vp = vpGO.AddComponent<VerletParticle>();
            vp.transform.position = interpolatedPoint;
            vp.previousPosition = vp.transform.position;
            vp.restPosition = x;
            rope.verletParticles.AddLast(vp);
            x += verletParticleSpacing;
        }
        return rope;
    }

    public static Rope CreateTautRope(Vector3 startPos, Vector3 endPos)
    {
        return CreateInterpolatedRope(new Vector3[] { startPos, endPos });
        /*
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
        */
    }

    public float ClampPositionToRopeExtents(float position)
    {
        return Mathf.Clamp(position, delta, RestLength - delta);
    }

    public void Detach(RopeAttachment ra)
    {
        ra.rope = null;
        ropeAttachments.Remove(ra);
    }

    public void Attach(RopeAttachment ra)
    {
        ra.rope = this;
        ra.restPosition = ClampPositionToRopeExtents(ClosestRestPositionOnRope(ra.AttachmentPoint()));
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
    
    private float ClosestRestPositionOnRope(Vector3 point)
    {
        IEnumerator<RopeNode> ropeNodes = GetAllRopeNodes();
        ropeNodes.MoveNext();
        RopeNode previousNode = ropeNodes.Current;
        float minSqrDist = Mathf.Infinity;
        float closestRestPosition = 0.0f;
        while (ropeNodes.MoveNext())
        {
            RopeNode currentNode = ropeNodes.Current;
            Vector3 s = previousNode.AttachmentPoint();
            Vector3 e = currentNode.AttachmentPoint();
            Vector3 v = e - s;
            float sqrMag = Vector3.SqrMagnitude(v);
            if(sqrMag > Mathf.Epsilon)
            {
                float t = Vector3.Dot(point - s, v) / sqrMag;
                t = Mathf.Clamp(t, 0, 1);
                Vector3 closestPointOnRope = s + t * v;
                float sqrDist = Vector3.SqrMagnitude(closestPointOnRope - point);
                if (sqrDist < minSqrDist)
                {
                    minSqrDist = sqrDist;
                    closestRestPosition = Mathf.Lerp(previousNode.restPosition, currentNode.restPosition, t);
                }
            }
        }
        return closestRestPosition;
    }

    private IEnumerator<RopeNode> GetAllRopeNodes()
    {
        LinkedListNode<VerletParticle> currentVPNode = verletParticles.First;
        LinkedListNode<RopeAttachment> currentRANode = ropeAttachments.First;
        while (currentVPNode != null)
        {
            if (currentRANode == null ||
                currentVPNode.Value.restPosition < currentRANode.Value.restPosition)
            {
                yield return currentVPNode.Value;
                currentVPNode = currentVPNode.Next;
            }
            else
            {
                yield return currentRANode.Value;
                currentRANode = currentRANode.Next;
            }
        }
    }

    public float RestPosition(IRopeNode ropeNode)
    {
        return restPositionMap[ropeNode];
    }

    public void SetRestPosition(IRopeAttachment ropeNode, float restPosition)
    {
        restPositionMap[ropeNode] = ClampPositionToRopeExtents(restPosition);
    }

    private void ApplyConstraint(VerletParticle vp1, VerletParticle vp2)
    {
        float constraintLength = RestPosition(vp2) - RestPosition(vp1);
        Vector3 x1 = vp1.AttachmentPoint();
        Vector3 x2 = vp2.AttachmentPoint();
        Vector3 d1 = x2 - x1;
        float d2 = d1.magnitude;
        if (d2 > Mathf.Epsilon)
        {
            float d3 = (d2 - constraintLength) / d2;
            Vector3 displacement = 0.5f * d1 * d3;
            vp1.ApplyDisplacement(displacement);
            vp2.ApplyDisplacement(-displacement);
            //vp1.transform.position = x1 + 0.5f * d1 * d3;
            //vp2.transform.position = x2 - 0.5f * d1 * d3;
        }
    }

    private void ApplyTension(IRopeAttachment ra1, IRopeAttachment ra2)
    {
        const float Ck = 0.1f;
        const float Cd = 0.1f;

        Vector3 x = ra2.AttachmentPoint() - ra1.AttachmentPoint();
        Vector3 direction = x.normalized;
        Vector3 x0 = (RestPosition(ra2) - RestPosition(ra1)) * direction;
        if (x.sqrMagnitude > x0.sqrMagnitude)
        {
            float m_red = 1.0f / (1 / ra1.Mass() + 1 / ra2.Mass());
            Vector3 v_rel = Vector3.Project(ra1.Velocity() - ra2.Velocity(), direction);
            float dt = Time.fixedDeltaTime;
            float k = m_red * Ck / (dt * dt);
            float b = m_red * Cd / dt;
            Vector3 f1 = k * (x - x0) - b * v_rel;
            Vector3 f2 = -k * (x - x0) + b * v_rel;
            ra1.ApplyForce(f1);
            ra2.ApplyForce(f2);
        }
    }

    private void OnDrawGizmos()
    {
        LinkedListNode<VerletParticle> currentVPNode = verletParticles.First;
        Gizmos.color = Color.blue;
        while (currentVPNode != null)
        {
            Gizmos.DrawSphere(currentVPNode.Value.AttachmentPoint(), 0.1f);
            currentVPNode = currentVPNode.Next;
        }
    }

}
