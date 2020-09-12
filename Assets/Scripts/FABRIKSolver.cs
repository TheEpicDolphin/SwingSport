using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeJoint
{
    public Vector3 position;
    public FreeJoint parent
    {
        get
        {
            return parent;
        }
        set
        {
            value.children.Add(this);
            parent = value;
        }
    }
    private List<FreeJoint> children;
    public int childCount
    {
        get
        {
            return children.Count;
        }
    }

    public FreeJoint()
    {

    }

    public FreeJoint GetChild(int i)
    {
        return children[i];
    }

    
}

public class FABRIKSolver
{
    const int maxIters = 25;
    const float tolerance = 1e-4f;
    private Transform root;
    private FreeJoint rootFreeJoint;
    private Dictionary<FreeJoint, float> jointLengthMap;
    private Dictionary<FreeJoint, Vector3> endEffectorToTargetMap;
    
    public FABRIKSolver(Transform root, Transform[] endEffectors, Vector3[] targets)
    {
        this.root = root;

        this.rootFreeJoint = new FreeJoint();
        CreateFreeJointTree(root, rootFreeJoint);
    }

    private void CreateFreeJointTree(Transform trans, FreeJoint joint)
    {
        for (int i = 0; i < trans.childCount; i++)
        {
            Transform childTrans = trans.GetChild(i);
            FreeJoint childJoint = new FreeJoint();
            childJoint.parent = joint;
            jointLengthMap[childJoint] = Vector3.Distance(childTrans.position, trans.position);
            CreateFreeJointTree(childTrans, childJoint);
        }
    }

    public void PlaceLimb()
    {

        List<Vector3> bonePosL = new List<Vector3>();
        List<Vector3> boneRelPosL = new List<Vector3>();
        for (int i = 0; i < joints.Length; i++)
        {
            bonePosL.Add(joints[i].position);
        }

        boneRelPosL.Add(joints[0].position);
        for (int i = 1; i < joints.Length; i++)
        {
            boneRelPosL.Add(joints[i - 1].InverseTransformPoint(joints[i].position));
        }

        List<Vector3> newBonePosL = FABRIKSolve(bonePosL);

        for (int i = 0; i < joints.Length - 1; i++)
        {
            Vector3 currentOffset = boneRelPosL[i + 1];
            Vector3 desiredOffset = joints[i].InverseTransformPoint(newBonePosL[i + 1]);
            joints[i].localRotation *= Quaternion.FromToRotation(currentOffset, desiredOffset);
        }
    }

    public List<Vector3> SolveTemp(List<Vector3> bonePos)
    {
        /*
         *  Uses FABRIK algorithm for solving IK 
         */
        Vector3 start = bonePos[0];
        int iters = 0;

        List<float> boneL = new List<float>();
        float totalLength = 0.0f;
        for (int i = 0; i < bonePos.Count - 1; i++)
        {
            float l = (bonePos[i + 1] - bonePos[i]).magnitude;
            boneL.Add(l);
            totalLength += l;
        }

        //Target is out of reach
        if (totalLength < (target.position - start).magnitude)
        {
            for (int i = 0; i < bonePos.Count - 1; i++)
            {
                Vector3 v = target.position - bonePos[i];
                bonePos[i + 1] = bonePos[i] + boneL[i] * v.normalized;

            }
            return bonePos;
        }

        while (iters < maxIters)
        {

            bonePos[bonePos.Count - 1] = target.position;
            for (int i = bonePos.Count - 1; i > 0; i--)
            {
                Vector3 v = bonePos[i - 1] - bonePos[i];
                bonePos[i - 1] = bonePos[i] + boneL[i - 1] * v.normalized;

            }

            bonePos[0] = start;
            for (int i = 0; i < bonePos.Count - 1; i++)
            {
                Vector3 v = bonePos[i + 1] - bonePos[i];
                bonePos[i + 1] = bonePos[i] + boneL[i] * v.normalized;

            }

            iters += 1;
        }
        return bonePos;
    }

    public void Solve()
    {
        int iters = 0;
        while (iters < maxIters)
        {
            BackwardReach(rootFreeJoint);
            ForwardReach(rootFreeJoint);
            iters += 1;
        }
        
    }

    private Vector3 BackwardReach(FreeJoint joint)
    {
        if (endEffectorToTargetMap.ContainsKey(joint))
        {
            /* This is an end effector, set it to the corresponding target position */
            joint.position = endEffectorToTargetMap[joint];
            ForwardReach(joint);
        }

        if (joint.childCount == 0)
        {
            /* This chain has no end effectors in it. Don't change anything */
            return joint.parent.position;
        }
        else
        {
            Vector3 centroid = Vector3.zero;
            for (int i = 0; i < joint.childCount; i++)
            {
                FreeJoint child = joint.GetChild(i);
                centroid += BackwardReach(child);
            }
            /* Taking average of new positions calculated by this joint's children */
            joint.position = centroid / joint.childCount;
        }
        Vector3 v = joint.parent.position - joint.position;
        Vector3 newParentPos = joint.position + jointLengthMap[joint] * v.normalized;
        return newParentPos;
    }

    private void ForwardReach(FreeJoint joint)
    {
        for (int i = 0; i < joint.childCount; i++)
        {
            FreeJoint child = joint.GetChild(i);
            Vector3 v = child.position - joint.position;
            Vector3 newChildPos = joint.position + jointLengthMap[child] * v.normalized;
            child.position = newChildPos;
            ForwardReach(child);
        }
    }

    public void Draw()
    {

    }
}
