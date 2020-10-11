using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Muscle : MonoBehaviour
{
    public ConfigurableJoint joint;
    public Rigidbody boneRb;
    public Transform animTarget;

    public JointDrive positionMatchingSpring = new JointDrive();

    Quaternion startLocalRotation;

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

        /*
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
        */

        startLocalRotation = transform.localRotation;
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

    public void MatchAnimationTargetPosition()
    {
        float Ck = 0.25f;
        float Cd = 0.25f;

        float m = boneRb.mass;
        float dt = Time.fixedDeltaTime;
        Vector3 targetToBone = boneRb.position - animTarget.position;
        Vector3 v = boneRb.velocity;
        Vector3 f = -(m * Ck / (dt * dt)) * targetToBone - (m * Cd / dt) * v;
        boneRb.AddForce(f);
    }

    public void MatchAnimationTargetRotation()
    {
        ConfigurableJointExtensions.SetTargetRotationLocal(joint, animTarget.localRotation, startLocalRotation);
    }

    private void FixedUpdate()
    {
        MatchAnimationTargetPosition();
        MatchAnimationTargetRotation();
    }

}
