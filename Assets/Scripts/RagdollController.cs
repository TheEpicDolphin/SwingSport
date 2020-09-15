using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollController : MonoBehaviour
{
    Transform animatedTargetRig;
    Transform ragdollRig;
    // Start is called before the first frame update
    void Start()
    {
        ragdollRig = transform.GetChild(0);
        CreateRagdoll(ragdollRig);
        animatedTargetRig = CreateAnimationTargetRig(ragdollRig);
        animatedTargetRig.parent = transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        MatchRagdollToAnimatedRig(ragdollRig, animatedTargetRig);
    }

    void CreateRagdoll(Transform bone)
    {
        Rigidbody boneRb = bone.gameObject.AddComponent<Rigidbody>();
        // Use this for now to avoid ragdoll falling
        //boneRb.useGravity = false;
        for(int i = 0; i < bone.childCount; i++)
        {
            Transform child = bone.GetChild(i);
            CreateRagdoll(child);
            ConfigurableJoint confJoint = child.gameObject.AddComponent<ConfigurableJoint>();
            confJoint.connectedBody = boneRb;
            confJoint.anchor = Vector3.zero;

            /*
            JointDrive jointXDrive = confJoint.xDrive;
            jointXDrive.positionSpring = 100.0f;
            confJoint.xDrive = jointXDrive;

            JointDrive jointYDrive = confJoint.yDrive;
            jointYDrive.positionSpring = 100.0f;
            confJoint.yDrive = jointYDrive;

            JointDrive jointZDrive = confJoint.zDrive;
            jointZDrive.positionSpring = 100.0f;
            confJoint.zDrive = jointZDrive;
            */
            JointDrive drive = new JointDrive();
            drive.positionSpring = 500.0f;
            drive.positionDamper = 0.0f;
            drive.maximumForce = 500.0f;

            SoftJointLimitSpring spring = new SoftJointLimitSpring();
            spring.spring = 0.0f;
            spring.damper = 0.0f;

            confJoint.slerpDrive = drive;
            confJoint.linearLimitSpring = spring;
            confJoint.rotationDriveMode = RotationDriveMode.Slerp;
            confJoint.projectionMode = JointProjectionMode.None;
            confJoint.targetAngularVelocity = Vector3.zero;
            confJoint.configuredInWorldSpace = false;
            confJoint.swapBodies = true;

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
    
}
