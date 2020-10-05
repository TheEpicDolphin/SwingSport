using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundedState : PlayerState
{
    public GroundedState(Player player)
    {
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

            player.surfaceConstrainer.connectedAnchor = hit.point;
            player.surfaceConstrainer.axis = hit.normal;
            player.surfaceConstrainer.restDistance = hit.point.y + player.activeRagdoll.AnimatedHipTargetY();

            player.activeRagdoll.MatchRotation(Quaternion.LookRotation(player.CameraRelativeInputDirection(), Vector3.up));
            return this;
        }
        else
        {
            player.surfaceConstrainer.enabled = false;
            player.activeRagdoll.animator.CrossFade("Falling", 0.1f);
            return new AerialState();
        }
    }

    public override PlayerState UpdateStep(Player player)
    {
        /* This belongs here because FixedUpdate would sometimes miss the spacebarDown event */
        if (player.input.spacebarDown)
        {
            /* jump */
            player.activeRagdoll.AddVelocityChange(10.0f * Vector3.up);

            // TODO: Delay for a bit to allow for player to take off ground
            player.activeRagdoll.animator.CrossFade("Falling", 0.25f);
            return new JumpingState(0.25f);
        }
        return this;
    }

    

    
}
