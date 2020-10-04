using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Muscle : MonoBehaviour
{
    public ConfigurableJoint joint;
    public Rigidbody boneRb;
    public Transform animTarget;

    public Transform ragdollRoot;
    public Transform animatedRigRoot;
    public JointDrive positionMatchingSpring = new JointDrive();

    private void Awake()
    {
        boneRb = GetComponent<Rigidbody>();

        joint = GetComponent<ConfigurableJoint>();
        if (!joint)
        {
            joint = gameObject.AddComponent<ConfigurableJoint>();
        }

        joint.anchor = Vector3.zero;
        joint.projectionMode = JointProjectionMode.PositionAndRotation;
        joint.enablePreprocessing = false;

        joint.xMotion = ConfigurableJointMotion.Free;
        joint.yMotion = ConfigurableJointMotion.Free;
        joint.zMotion = ConfigurableJointMotion.Free;
        joint.angularXMotion = ConfigurableJointMotion.Free;
        joint.angularYMotion = ConfigurableJointMotion.Free;
        joint.angularZMotion = ConfigurableJointMotion.Free;

        JointDrive drive = new JointDrive();
        drive.positionSpring = 10000.0f;
        drive.positionDamper = 100.0f;
        drive.maximumForce = float.MaxValue;

        joint.angularXDrive = drive;
        joint.angularYZDrive = drive;
        joint.targetAngularVelocity = Vector3.zero;


        SoftJointLimit lowAngXLim = joint.lowAngularXLimit;
        lowAngXLim.limit = -120.0f;
        SoftJointLimit highAngXLim = joint.highAngularXLimit;
        highAngXLim.limit = 120.0f;
        joint.lowAngularXLimit = lowAngXLim;
        joint.highAngularXLimit = highAngXLim;

        SoftJointLimit angYLim = joint.angularYLimit;
        angYLim.limit = 120.0f;
        joint.angularYLimit = angYLim;

        SoftJointLimit angZLim = joint.angularZLimit;
        angZLim.limit = 120.0f;
        joint.angularZLimit = angZLim;
    }

    public void SetParent(Muscle parentMuscle)
    {
        joint.xMotion = ConfigurableJointMotion.Locked;
        joint.yMotion = ConfigurableJointMotion.Locked;
        joint.zMotion = ConfigurableJointMotion.Locked;
        joint.connectedBody = parentMuscle.boneRb;
    }

    public void SetAnimationTarget(Transform animTarget)
    {
        this.animTarget = animTarget;
    }

    public void MatchAnimationTarget()
    {
        if (joint.connectedBody)
        {
            joint.targetRotation = Quaternion.Inverse(animTarget.localRotation);
        }
        else
        {
            joint.targetRotation = Quaternion.Inverse(animTarget.localRotation) * boneRb.rotation;
        }
    }

    private void FixedUpdate()
    {
        //Vector3 relRagdollBonePos = ragdollRoot.InverseTransformPoint(boneRb.position);
        //Vector3 relAnimTargetPos = animatedRigRoot.InverseTransformPoint(animTarget.position);
        //Vector3 f = positionMatchingSpring.positionSpring * (relAnimTargetPos - relRagdollBonePos)
        //                - positionMatchingSpring.positionDamper * (boneRb.velocity);
        //boneRb.AddForce(f);
    }

    private void LateUpdate()
    {
        MatchAnimationTarget();
    }


}
