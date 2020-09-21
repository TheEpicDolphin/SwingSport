using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveRagdoll : MonoBehaviour
{
    Transform animatedTargetRigHip;
    Rigidbody hipRb;
    ConfigurableJoint hipConfJoint;
    public bool useGravity = true;

    public Vector3 Velocity
    {
        get
        {
            return hipRb.velocity;
        }
    }

    float totalMass = 50.0f;
    float boneMass;

    private void Awake()
    {
        Transform ragdollRigHip = transform;
        boneMass = totalMass / CountBones(ragdollRigHip);

        ConvertRigToRagdoll(ragdollRigHip);
        animatedTargetRigHip = CreateAnimationTargetRig(ragdollRigHip);
        animatedTargetRigHip.parent = transform.parent;

        /* Set hip configurable joint */
        hipConfJoint = ragdollRigHip.gameObject.AddComponent<ConfigurableJoint>();

        hipConfJoint.projectionMode = JointProjectionMode.PositionAndRotation;
        hipConfJoint.enablePreprocessing = false;

        hipConfJoint.xMotion = ConfigurableJointMotion.Free;
        hipConfJoint.yMotion = ConfigurableJointMotion.Free;
        hipConfJoint.zMotion = ConfigurableJointMotion.Free;
        hipConfJoint.angularXMotion = ConfigurableJointMotion.Free;
        hipConfJoint.angularYMotion = ConfigurableJointMotion.Free;
        hipConfJoint.angularZMotion = ConfigurableJointMotion.Free;

        JointDrive hipJointAngularDrive = new JointDrive();
        hipJointAngularDrive.positionSpring = 10000.0f;
        hipJointAngularDrive.positionDamper = 100.0f;
        hipJointAngularDrive.maximumForce = float.MaxValue;

        hipConfJoint.angularXDrive = hipJointAngularDrive;
        hipConfJoint.angularYZDrive = hipJointAngularDrive;

        hipRb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void LateUpdate()
    {
        MatchRagdollToAnimatedRig(transform, animatedTargetRigHip);
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
        boneRb.drag = 0.0f;
        boneRb.angularDrag = 0.0f;
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
            confJoint.enablePreprocessing = false;
            
            confJoint.xMotion = ConfigurableJointMotion.Locked;
            confJoint.yMotion = ConfigurableJointMotion.Locked;
            confJoint.zMotion = ConfigurableJointMotion.Locked;
            confJoint.angularXMotion = ConfigurableJointMotion.Free;
            confJoint.angularYMotion = ConfigurableJointMotion.Free;
            confJoint.angularZMotion = ConfigurableJointMotion.Free;
            
            JointDrive drive = new JointDrive();
            drive.positionSpring = 10000.0f;
            drive.positionDamper = 100.0f;
            drive.maximumForce = float.MaxValue;

            confJoint.angularXDrive = drive;
            confJoint.angularYZDrive = drive;
            confJoint.targetAngularVelocity = Vector3.zero;

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
        for (int i = 0; i < animBone.childCount; i++)
        {
            Transform ragdollBoneChild = ragdollBone.GetChild(i);
            Transform animeBoneChild = animBone.GetChild(i);
            ConfigurableJoint confJoint = ragdollBoneChild.GetComponent<ConfigurableJoint>();
            confJoint.targetRotation = animeBoneChild.localRotation;
            MatchRagdollToAnimatedRig(ragdollBoneChild, animeBoneChild);
        }
    }

    public void AddAcceleration(Vector3 acceleration)
    {
        hipRb.AddForce(totalMass * acceleration, ForceMode.Force);
    }

    public void AddVelocityChange(Vector3 velocityChange)
    {
        hipRb.AddForce(totalMass * velocityChange, ForceMode.Impulse);
    }

    public void MatchRotation(Quaternion targetRotation)
    {
        hipConfJoint.targetRotation = Quaternion.Inverse(targetRotation);
    }
}
