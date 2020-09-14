using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimController : MonoBehaviour
{
    Transform character;

    /* determines whether the player will look in the direction of the camera or not */
    public bool isCharacterLockedToCamera = true;

    // Start is called before the first frame update
    void Start()
    {
        character = transform.Find("Character");
    }

    public void OrientOnGround(Transform view)
    {
        if (isCharacterLockedToCamera)
        {
            // Orient character to direction of view (Last of Us style)
            character.rotation = view.rotation;
        }
        else
        {
            // Orient character to direction of motion (Uncharted style)
        }
    }

    
    public void OrientInAir(Transform view, Vector3 orientingForce)
    {
        Debug.Log(orientingForce.magnitude);
        if (orientingForce.magnitude > 100.0f)
        {
            Vector3 forwardSphereProj = Vector3.ProjectOnPlane(view.forward, orientingForce).normalized;
            if (Vector3.Angle(view.forward, forwardSphereProj) < Vector3.Angle(view.forward, -forwardSphereProj))
            {
                character.rotation = Quaternion.LookRotation(forwardSphereProj, -orientingForce.normalized);
            }
            else
            {
                character.rotation = Quaternion.LookRotation(-forwardSphereProj, -orientingForce.normalized);
            }
        }
        else
        {
            Quaternion desiredUprightRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(view.forward, Vector3.up), Vector3.up);
            character.rotation = Quaternion.Slerp(character.rotation, desiredUprightRotation, 2.0f * Time.deltaTime);
        }
    }
}
