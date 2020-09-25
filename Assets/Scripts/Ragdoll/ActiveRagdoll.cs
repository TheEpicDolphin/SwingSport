using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveRagdoll : MonoBehaviour
{
    List<Muscle> ragdollMuscles = new List<Muscle>();

    /* Animated rig that the active ragdoll tries to copy */
    Transform animatedTargetRigHip;

    /* Rigidbody of hip */
    Rigidbody hipRb;

    /* hip joint */
    ConfigurableJoint hipConfJoint;

    /* Determines if active ragdoll is affected by gravity */
    public bool useGravity = true;

    float airDrag = 0.5f;

    float maxSpeed = 60.0f;

    public Vector3 Velocity
    {
        get
        {
            /* Return the hip velocity when fetching ragdoll velocity */
            return hipRb.velocity;
        }
        set
        {
            /* Sets velocity of each ragdoll joint to the value */
            /* **CAUTION** Moving ragdoll by setting velocities can 
             * be visually jarring and can create insanely large collision forces*/
            foreach(Muscle muscle in ragdollMuscles)
            {
                muscle.bone.velocity = value;
            }
        }
    }

    /* total mass of the ragdoll */
    float totalMass = 50.0f;

    /* mass of each bone */
    float boneMass;

    private void Awake()
    {
        animatedTargetRigHip = CreateAnimatedTargetRig(transform);
        animatedTargetRigHip.parent = transform.parent;

        boneMass = totalMass / CountBones(transform);

        CreateAndConnectMuscles(transform, animatedTargetRigHip);

        hipRb = GetComponent<Rigidbody>();
        hipConfJoint = gameObject.GetComponent<ConfigurableJoint>();
    }

    private void LateUpdate()
    {
        MatchRagdollToAnimatedRig();
    }

    int CountBones(Transform bone)
    {
        int count = 1;
        for (int i = 0; i < bone.childCount; i++)
        {
            count += CountBones(bone.GetChild(i));
        }
        return count;
    }

    List<Transform> FlattenedBoneHeirarchy(Transform bone)
    {
        List<Transform> flattened = new List<Transform>() { bone };
        for(int i = 0; i < bone.childCount; i++)
        {
            flattened.AddRange(FlattenedBoneHeirarchy(bone.GetChild(i)));
        }
        return flattened;
    }

    Transform CreateAnimatedTargetRig(Transform bone)
    {
        GameObject animBoneGO = new GameObject();
        Transform animBone = animBoneGO.transform;
        animBone.position = bone.transform.position;
        animBone.rotation = bone.transform.rotation;
        for (int i = 0; i < bone.childCount; i++)
        {
            Transform child = bone.GetChild(i);
            Transform childAnimBone = CreateAnimatedTargetRig(child);
            childAnimBone.parent = animBone;
        }
        return animBone;
    }

    Muscle CreateAndConnectMuscles(Transform ragdollBone, Transform animBone)
    {
        Rigidbody boneRb = ragdollBone.GetComponent<Rigidbody>();
        if (!boneRb)
        {
            boneRb = ragdollBone.gameObject.AddComponent<Rigidbody>();
        }
        boneRb.mass = boneMass;
        boneRb.drag = 0.0f;
        boneRb.angularDrag = 0.0f;
        boneRb.useGravity = useGravity;

        Muscle muscle = new Muscle(boneRb, animBone);
        for (int i = 0; i < ragdollBone.childCount; i++)
        {
            Transform childRagdollBone = ragdollBone.GetChild(i);
            Transform childAnimBone = animBone.GetChild(i);
            Muscle childMuscle = CreateAndConnectMuscles(childRagdollBone, childAnimBone);
            childMuscle.SetParent(muscle);
        }
        this.ragdollMuscles.Add(muscle);
        return muscle;
    }

    void MatchRagdollToAnimatedRig()
    {
        foreach(Muscle muscle in ragdollMuscles)
        {
            muscle.MatchAnimationTarget();
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

    /* Sets the position of the ragdoll hip to pos */
    /* **CAUTION** Not recommended to use this to move active ragdoll because 
     * translating rigidbodies can cause jitter and insanely large collision forces*/
    public void MovePosition(Vector3 pos)
    {
        //TODO: make this move the whole ragdoll so that the current relative
        // positions and rotations of the non-hip bones are preserved and the 
        // hip is moved to pos
        hipRb.MovePosition(pos);
    }

    public void ApplyAirDrag()
    {
        foreach (Muscle muscle in ragdollMuscles)
        {
            Vector3 curVel = muscle.bone.velocity;
            float curSpeed = curVel.magnitude;
            muscle.bone.AddForce(-airDrag * Mathf.Max(0.0f, curSpeed - maxSpeed) * curVel);
        }
    }

}
