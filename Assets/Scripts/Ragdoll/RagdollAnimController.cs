using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollAnimController : MonoBehaviour
{
    Transform animatedTargetRigHip;
    Transform ragdollRigHip;

    float totalMass = 50.0f;
    float boneMass;
    bool useGravity = true;

    private void Awake()
    {
        ragdollRigHip = transform.GetChild(0);
        boneMass = totalMass / CountBones(ragdollRigHip);

        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        rb.useGravity = useGravity;
        ConfigurableJoint rootConfJoint = gameObject.AddComponent<ConfigurableJoint>();

        ConvertRigToRagdoll(ragdollRigHip);
        animatedTargetRigHip = CreateAnimationTargetRig(ragdollRigHip);
        animatedTargetRigHip.parent = transform.parent;

        /* Set hip configurable joint */
        ConfigurableJoint hipConfJoint = ragdollRigHip.gameObject.AddComponent<ConfigurableJoint>();
        hipConfJoint.connectedBody = rb;
        hipConfJoint.anchor = Vector3.zero;

        hipConfJoint.projectionMode = JointProjectionMode.PositionAndRotation;

        hipConfJoint.xMotion = ConfigurableJointMotion.Free;
        hipConfJoint.yMotion = ConfigurableJointMotion.Free;
        hipConfJoint.zMotion = ConfigurableJointMotion.Free;
        hipConfJoint.angularXMotion = ConfigurableJointMotion.Free;
        hipConfJoint.angularYMotion = ConfigurableJointMotion.Free;
        hipConfJoint.angularZMotion = ConfigurableJointMotion.Free;

        // TODO: SET MAXIMUM FORCE AS MAXIMUM FLOAT
        JointDrive hipJointAngularDrive = new JointDrive();
        hipJointAngularDrive.positionSpring = 10000.0f;
        hipJointAngularDrive.positionDamper = 100.0f;
        hipJointAngularDrive.maximumForce = float.MaxValue;

        JointDrive hipJointLinearDrive = new JointDrive();
        hipJointLinearDrive.positionSpring = 10000.0f;
        hipJointLinearDrive.positionDamper = 100.0f;
        hipJointLinearDrive.maximumForce = float.MaxValue;

        hipConfJoint.xDrive = hipJointLinearDrive;
        hipConfJoint.yDrive = hipJointLinearDrive;
        hipConfJoint.zDrive = hipJointLinearDrive;
        hipConfJoint.angularXDrive = hipJointAngularDrive;
        hipConfJoint.angularYZDrive = hipJointAngularDrive;

        /* Set root configurable joint */
        rootConfJoint.projectionMode = JointProjectionMode.PositionAndRotation;

        rootConfJoint.xMotion = ConfigurableJointMotion.Free;
        rootConfJoint.yMotion = ConfigurableJointMotion.Free;
        rootConfJoint.zMotion = ConfigurableJointMotion.Free;
        rootConfJoint.angularXMotion = ConfigurableJointMotion.Free;
        rootConfJoint.angularYMotion = ConfigurableJointMotion.Free;
        rootConfJoint.angularZMotion = ConfigurableJointMotion.Free;

        JointDrive rootJointDrive = new JointDrive();
        rootJointDrive.positionSpring = 10000.0f;
        rootJointDrive.positionDamper = 100.0f;
        rootJointDrive.maximumForce = float.MaxValue;

        rootConfJoint.angularXDrive = rootJointDrive;
        rootConfJoint.angularYZDrive = rootJointDrive;

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void LateUpdate()
    {
        MatchRagdollToAnimatedRig(ragdollRigHip, animatedTargetRigHip);
    }

    int CountBones(Transform bone)
    {
        int count = 1;
        for(int i = 0; i < bone.childCount; i++)
        {
            count += CountBones(bone.GetChild(i));
        }
        return count;
    }

    void ConvertRigToRagdoll(Transform bone)
    {
        Rigidbody boneRb = bone.GetComponent<Rigidbody>();
        if (!boneRb)
        {
            boneRb = bone.gameObject.AddComponent<Rigidbody>();
        }
        boneRb.mass = boneMass;
        boneRb.useGravity = useGravity;
        for (int i = 0; i < bone.childCount; i++)
        {
            Transform child = bone.GetChild(i);
            ConvertRigToRagdoll(child);
            ConfigurableJoint confJoint = child.GetComponent<ConfigurableJoint>();
            if (!confJoint)
            {
                confJoint = child.gameObject.AddComponent<ConfigurableJoint>();
            }
            
            confJoint.connectedBody = boneRb;
            confJoint.anchor = Vector3.zero;

            confJoint.projectionMode = JointProjectionMode.PositionAndRotation;

            JointDrive drive = new JointDrive();
            drive.positionSpring = 10000.0f;
            drive.positionDamper = 100.0f;
            drive.maximumForce = float.MaxValue;

            confJoint.angularXDrive = drive;
            confJoint.angularYZDrive = drive;
            confJoint.targetAngularVelocity = Vector3.zero;

            confJoint.xMotion = ConfigurableJointMotion.Locked;
            confJoint.yMotion = ConfigurableJointMotion.Locked;
            confJoint.zMotion = ConfigurableJointMotion.Locked;
            confJoint.angularXMotion = ConfigurableJointMotion.Free;
            confJoint.angularYMotion = ConfigurableJointMotion.Free;
            confJoint.angularZMotion = ConfigurableJointMotion.Free;

            SoftJointLimit lowAngXLim = confJoint.lowAngularXLimit;
            lowAngXLim.limit = -120.0f;
            SoftJointLimit highAngXLim = confJoint.highAngularXLimit;
            highAngXLim.limit = 120.0f;
            confJoint.lowAngularXLimit = lowAngXLim;
            confJoint.highAngularXLimit = highAngXLim;

            SoftJointLimit angYLim = confJoint.angularYLimit;
            angYLim.limit = 120.0f;
            confJoint.angularYLimit = angYLim;

            SoftJointLimit angZLim = confJoint.angularZLimit;
            angZLim.limit = 120.0f;
            confJoint.angularZLimit = angZLim;


        }
    }


    Transform CreateAnimationTargetRig(Transform bone)
    {
        GameObject animBoneGO = new GameObject();
        Transform animBone = animBoneGO.transform;
        animBone.position = bone.transform.position;
        animBone.rotation = bone.transform.rotation;
        for (int i = 0; i < bone.childCount; i++)
        {
            Transform child = bone.GetChild(i);
            Transform childAnimBone = CreateAnimationTargetRig(child);
            childAnimBone.parent = animBone;
        }
        return animBone;
    }

    void MatchRagdollToAnimatedRig(Transform ragdollBone, Transform animBone)
    {
        //ConfigurableJoint confJoint = ragdollBone.GetComponent<ConfigurableJoint>();
        //confJoint.targetRotation = animBone.localRotation;
        for (int i = 0; i < animBone.childCount; i++)
        {
            Transform ragdollBoneChild = ragdollBone.GetChild(i);
            Transform animeBoneChild = animBone.GetChild(i);
            ConfigurableJoint confJoint = ragdollBoneChild.GetComponent<ConfigurableJoint>();
            confJoint.targetRotation = animeBoneChild.localRotation;
            MatchRagdollToAnimatedRig(ragdollBoneChild, animeBoneChild);
        }
    }
}
