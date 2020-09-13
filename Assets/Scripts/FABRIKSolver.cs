using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeJoint
{
    public Vector3 position;
    private FreeJoint parent;
    public FreeJoint Parent
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

    public FreeJoint(Vector3 position)
    {
        this.position = position;
        this.children = new List<FreeJoint>();
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
    private Transform rootTrans;
    private FreeJoint root;
    private Dictionary<FreeJoint, float> jointLengthMap;
    private Dictionary<FreeJoint, Vector3> endEffectorToTargetMap;
    
    public FABRIKSolver(Transform rootTrans, Dictionary<Transform, Vector3> endEffectorTransToTargetMap)
    {
        this.rootTrans = rootTrans;
        this.jointLengthMap = new Dictionary<FreeJoint, float>();
        this.endEffectorToTargetMap = new Dictionary<FreeJoint, Vector3>();
        this.root = CreateFreeJointTree(rootTrans, endEffectorTransToTargetMap);
    }

    private FreeJoint CreateFreeJointTree(Transform trans, Dictionary<Transform, Vector3> endEffectorTransToTargetMap)
    {
        FreeJoint joint = new FreeJoint(trans.position);
        for (int i = 0; i < trans.childCount; i++)
        {
            Transform childTrans = trans.GetChild(i);
            FreeJoint childJoint = CreateFreeJointTree(childTrans, endEffectorTransToTargetMap);
            childJoint.Parent = joint;
            jointLengthMap[childJoint] = Vector3.Distance(childTrans.position, trans.position);
            if (endEffectorTransToTargetMap.ContainsKey(childTrans))
            {
                endEffectorToTargetMap[childJoint] = endEffectorTransToTargetMap[childTrans];
            }
             
        }
        return joint;
    }

    public void Solve()
    {
        int iters = 0;
        while (iters < maxIters)
        {
            BackwardReach(root);
            root.position = rootTrans.transform.position;
            ForwardReach(root);
            iters += 1;
        }
        SetBonesTransformsToFitJoints(rootTrans, root);
    }

    private Vector3 BackwardReach(FreeJoint joint)
    {
        if (endEffectorToTargetMap.ContainsKey(joint))
        {
            /* This is an end effector, set it to the corresponding target position */
            joint.position = endEffectorToTargetMap[joint];
            ForwardReach(joint);
        }
        else if (joint.childCount == 0)
        {
            /* This chain has no end effectors in it. Don't change anything */
            return joint.Parent.position;
        }
        else
        {
            Vector3 centroid = Vector3.zero;
            for (int i = 0; i < joint.childCount; i++)
            {
                FreeJoint child = joint.GetChild(i);
                centroid += BackwardReach(child);
            }
            /* Set joint position as centroid of new positions calculated by this joint's children */
            joint.position = centroid / joint.childCount;
        }
        if(joint.Parent != null)
        {
            Vector3 v = joint.Parent.position - joint.position;
            Vector3 newParentPos = joint.position + jointLengthMap[joint] * v.normalized;
            return newParentPos;
        }
        return Vector3.zero;
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

    private void SetBonesTransformsToFitJoints(Transform trans, FreeJoint joint)
    {
        /*
        for (int i = 0; i < trans.childCount; i++)
        {
            Transform childTrans = trans.GetChild(i);
            FreeJoint childJoint = joint.GetChild(i);
            Vector3 currentOffset = trans.InverseTransformPoint(childTrans.position);
            Vector3 desiredOffset = trans.InverseTransformPoint(childJoint.position);
            trans.localRotation *= Quaternion.FromToRotation(currentOffset, desiredOffset);
            SetBonesTransformsToFitJoints(childTrans, childJoint);
        }
        */

        Vector3 childrenCentroid = Vector3.zero;

        if(trans.childCount == 1)
        {
            Transform childTrans = trans.GetChild(0);
            FreeJoint childJoint = joint.GetChild(0);
            Vector3 currentOffset = trans.InverseTransformPoint(childTrans.position);
            Vector3 desiredOffset = trans.InverseTransformPoint(childJoint.position);
            trans.localRotation *= Quaternion.FromToRotation(currentOffset, desiredOffset);
            SetBonesTransformsToFitJoints(childTrans, childJoint);
        }
        else
        {
            for (int i = 0; i < trans.childCount; i++)
            {
                Transform childTrans = trans.GetChild(i);
                FreeJoint childJoint = joint.GetChild(i);
                SetBonesTransformsToFitJoints(childTrans, childJoint);
            }
        }
    }

    public void Draw()
    {

    }
}
