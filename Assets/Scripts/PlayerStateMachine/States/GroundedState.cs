using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundedState : PlayerState
{
    public GroundedState(Player player)
    {
        player.activeRagdoll.animator.CrossFade("GroundedMovement", 0.1f);
        player.surfaceConstrainer.enabled = true;
    }

    public override void OnEnter()
    {
        throw new System.NotImplementedException();
    }

    public override PlayerState FixedUpdateStep(Player player)
    {
        RaycastHit hit;
        /* Checks if player is on the ground. Consider doing a spherecast for more accuracy */
        if (Physics.Raycast(player.transform.position, Vector3.down, out hit, 1.3f, ~LayerMask.GetMask("Player")))
        {
            Vector3 vDesired = player.groundMovementSpeed * player.CameraRelativeInputDirection();
            Vector3 a = 10.0f * (vDesired - player.activeRagdoll.Velocity);
            a = Vector3.ClampMagnitude(a, 100.0f);
            player.activeRagdoll.AddAcceleration(a);

            /* Rotating character is done in RagdollAnimController */
            //Vector3 turningTorque = 100.0f * Vector3.Cross(transform.forward, cameraTrans.forward);
            //rb.AddTorque(turningTorque, ForceMode.Acceleration);

            /* Rotates player to face in direction of camera */
            //player.activeRagdoll.MatchRotation(player.playerCamera.transform.rotation);

            player.surfaceConstrainer.planePoint = hit.point;
            player.surfaceConstrainer.normal = hit.normal;
            player.surfaceConstrainer.distance = 2.5f * (hit.point.y + player.activeRagdoll.AnimatedHipTargetY());

            Vector3 movementDir = player.CameraRelativeInputDirection();
            if(movementDir.magnitude > 1e-4f)
            {
                player.activeRagdoll.MatchRotation(Quaternion.LookRotation(movementDir, Vector3.up));
            }
            return this;
        }
        else
        {
            player.surfaceConstrainer.enabled = false;
            return new AerialState(player);
        }
    }

    public override PlayerState UpdateStep(Player player)
    {
        /* This belongs here because FixedUpdate would sometimes miss the spacebarDown event */
        if (player.input.spacebarDown)
        {
            player.surfaceConstrainer.enabled = false;
            // TODO: Delay for a bit to allow for player to take off ground
            player.activeRagdoll.animator.CrossFade("Falling", 0.1f);
            return new JumpingState(0.1f);
        }
        Vector3 groundNormal = player.surfaceConstrainer.normal;
        float groundSpeed = Vector3.ProjectOnPlane(player.activeRagdoll.Velocity, groundNormal).magnitude;
        player.activeRagdoll.animator.SetFloat("GroundSpeed", groundSpeed / player.groundMovementSpeed);
        return this;
    }

    

    
}
