using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollAnimController : MonoBehaviour
{
    Transform animatedTargetRig;
    Transform ragdollRig;

    private void Awake()
    {
        ragdollRig = transform.GetChild(0);
        CreateRagdoll(ragdollRig);
        animatedTargetRig = CreateAnimationTargetRig(ragdollRig);
        animatedTargetRig.parent = transform;

        ConfigurableJoint confJoint = ragdollRig.gameObject.AddComponent<ConfigurableJoint>();

        confJoint.xMotion = ConfigurableJointMotion.Free;
        confJoint.yMotion = ConfigurableJointMotion.Free;
        confJoint.zMotion = ConfigurableJointMotion.Free;
        confJoint.angularXMotion = ConfigurableJointMotion.Free;
        confJoint.angularYMotion = ConfigurableJointMotion.Free;
        confJoint.angularZMotion = ConfigurableJointMotion.Free;

        JointDrive drive = new JointDrive();
        drive.positionSpring = 500.0f;
        drive.positionDamper = 0.0f;
        drive.maximumForce = 500.0f;

        confJoint.angularXDrive = drive;
        confJoint.angularYZDrive = drive;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        ConfigurableJoint confJoint = ragdollRig.GetComponent<ConfigurableJoint>();
        confJoint.targetRotation = Quaternion.Inverse(Camera.main.transform.rotation);
        MatchRagdollToAnimatedRig(ragdollRig, animatedTargetRig);
    }

    private void FixedUpdate()
    {
        Rigidbody hipRb = ragdollRig.GetComponent<Rigidbody>();
        
    }

    void CreateRagdoll(Transform bone)
    {
        Rigidbody boneRb = bone.gameObject.AddComponent<Rigidbody>();
        for(int i = 0; i < bone.childCount; i++)
        {
            Transform child = bone.GetChild(i);
            CreateRagdoll(child);
            ConfigurableJoint confJoint = child.gameObject.AddComponent<ConfigurableJoint>();
            confJoint.connectedBody = boneRb;
            confJoint.anchor = Vector3.zero;

            JointDrive drive = new JointDrive();
            drive.positionSpring = 500.0f;
            drive.positionDamper = 0.0f;
            drive.maximumForce = 500.0f;

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
            confJoint.targetRotation = animeBoneChild.localRotation;//Quaternion.Inverse(animeBoneChild.rotation) * new Quaternion();
            MatchRagdollToAnimatedRig(ragdollBoneChild, animeBoneChild);
        }
    }

    void RotateRagdollToFaceDirection(Vector3 direction, Vector3 up)
    {

    }
}
