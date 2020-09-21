using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Muscle
{
    public ConfigurableJoint joint;
    public Rigidbody bone;
    Transform animTarget;

    /*
    private List<Muscle> children;
    private Muscle parent;
    public Muscle Parent
    {
        get
        {
            return parent;
        }
        set
        {
            if (value != null)
            {
                joint.xMotion = ConfigurableJointMotion.Locked;
                joint.yMotion = ConfigurableJointMotion.Locked;
                joint.zMotion = ConfigurableJointMotion.Locked;
                joint.connectedBody = value.bone;
                value.children.Add(this);
                parent = value;
            }
            else
            {
                joint.xMotion = ConfigurableJointMotion.Free;
                joint.yMotion = ConfigurableJointMotion.Free;
                joint.zMotion = ConfigurableJointMotion.Free;
                joint.connectedBody = null;
                if (parent != null)
                {
                    parent.children.Remove(this);
                }
                parent = null;
            }
        }
    }
    */

    public Muscle(Rigidbody bone, Transform animTarget)
    {
        this.bone = bone;
        this.animTarget = animTarget;

        joint = bone.gameObject.GetComponent<ConfigurableJoint>();
        if (!joint)
        {
            joint = bone.gameObject.AddComponent<ConfigurableJoint>();
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
        joint.connectedBody = parentMuscle.bone;
    }

    public void MatchAnimationTarget()
    {
        if (joint.connectedBody)
        {
            joint.targetRotation = Quaternion.Inverse(animTarget.localRotation);
        }
        else
        {
            joint.targetRotation = Quaternion.Inverse(animTarget.localRotation) * bone.rotation;
        }
    }
}
