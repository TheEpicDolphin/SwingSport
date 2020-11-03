using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveRagdoll : MonoBehaviour
{
    List<Muscle> ragdollMuscles = new List<Muscle>();

    /* Determines if active ragdoll is affected by gravity */
    bool useGravity = false;

    public void CreateActiveRagdoll(Transform animatedRigHip, float totalMass)
    {
        float boneMass = totalMass / CountBones(transform);
        CreateAndConnectMuscles(transform, animatedRigHip, boneMass);
    }

    int CountBones(Transform bone)
    {
        if (bone.tag != "RagdollBone")
        {
            return 0;
        }
        int count = 1;
        for (int i = 0; i < bone.childCount; i++)
        {
            count += CountBones(bone.GetChild(i));
        }
        return count;
    }

    void CreateAndConnectMuscles(Transform ragdollBone, Transform animBone, float boneMass)
    {
        if(ragdollBone.tag == "RagdollBone")
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

            Muscle muscle = ragdollBone.gameObject.AddComponent<Muscle>();
            muscle.IsPinned = true;
            muscle.SetAnimationTarget(animBone);
            Muscle parentMuscle = ragdollBone.parent.GetComponent<Muscle>();
            if (parentMuscle)
            {
                muscle.SetParent(parentMuscle);
            }
            this.ragdollMuscles.Add(muscle);

            for (int i = 0; i < animBone.childCount; i++)
            {
                Transform childRagdollBone = ragdollBone.GetChild(i);
                Transform childAnimBone = animBone.GetChild(i);
                CreateAndConnectMuscles(childRagdollBone, childAnimBone, boneMass);
            }
        }
    }

}
