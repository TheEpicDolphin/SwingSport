using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollAnimController : MonoBehaviour
{
    Transform animatedTargetRigHip;
    Transform ragdollRigHip;

    private void Awake()
    {
        ragdollRigHip = transform.GetChild(0);
        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        rb.useGravity = false;
        ConfigurableJoint rootConfJoint = gameObject.AddComponent<ConfigurableJoint>();

        ConvertRigToRagdoll(ragdollRigHip);
        animatedTargetRigHip = CreateAnimationTargetRig(ragdollRigHip);
        animatedTargetRigHip.parent = transform.parent;

        /* Set hip configurable joint */
        ConfigurableJoint hipConfJoint = ragdollRigHip.gameObject.AddComponent<ConfigurableJoint>();
        hipConfJoint.connectedBody = rb;
        hipConfJoint.anchor = Vector3.zero;

        hipConfJoint.xMotion = ConfigurableJointMotion.Free;
        hipConfJoint.yMotion = ConfigurableJointMotion.Free;
        hipConfJoint.zMotion = ConfigurableJointMotion.Free;
        hipConfJoint.angularXMotion = ConfigurableJointMotion.Free;
        hipConfJoint.angularYMotion = ConfigurableJointMotion.Free;
        hipConfJoint.angularZMotion = ConfigurableJointMotion.Free;

        JointDrive hipJointDrive = new JointDrive();
        hipJointDrive.positionSpring = 1000.0f;
        hipJointDrive.positionDamper = 100.0f;
        hipJointDrive.maximumForce = 1000.0f;

        hipConfJoint.xDrive = hipJointDrive;
        hipConfJoint.yDrive = hipJointDrive;
        hipConfJoint.zDrive = hipJointDrive;
        hipConfJoint.angularXDrive = hipJointDrive;
        hipConfJoint.angularYZDrive = hipJointDrive;

        /* Set root configurable joint */
        rootConfJoint.xMotion = ConfigurableJointMotion.Free;
        rootConfJoint.yMotion = ConfigurableJointMotion.Free;
        rootConfJoint.zMotion = ConfigurableJointMotion.Free;
        rootConfJoint.angularXMotion = ConfigurableJointMotion.Free;
        rootConfJoint.angularYMotion = ConfigurableJointMotion.Free;
        rootConfJoint.angularZMotion = ConfigurableJointMotion.Free;

        JointDrive rootJointDrive = new JointDrive();
        rootJointDrive.positionSpring = 1000.0f;
        rootJointDrive.positionDamper = 100.0f;
        rootJointDrive.maximumForce = 1000.0f;

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

    void ConvertRigToRagdoll(Transform bone)
    {
        Rigidbody boneRb = GetComponent<Rigidbody>();
        if (!boneRb)
        {
            boneRb = bone.gameObject.AddComponent<Rigidbody>();
        }

        boneRb.useGravity = false;
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

            JointDrive drive = new JointDrive();
            drive.positionSpring = 1000.0f;
            drive.positionDamper = 100.0f;
            drive.maximumForce = 1000.0f;

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
        for (int i = 0; i < ragdollBone.childCount; i++)
        {
            Transform ragdollBoneChild = ragdollBone.GetChild(i);
            Transform animeBoneChild = animBone.GetChild(i);
            ConfigurableJoint confJoint = ragdollBoneChild.GetComponent<ConfigurableJoint>();
            confJoint.targetRotation = animeBoneChild.localRotation;
            MatchRagdollToAnimatedRig(ragdollBoneChild, animeBoneChild);
        }
    }
}
