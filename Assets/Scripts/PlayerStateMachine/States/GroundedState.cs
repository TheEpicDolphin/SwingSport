using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundedState : PlayerState
{
    public GroundedState(PlayerStateMachine playerSM, Player player) : base(playerSM, player)
    {

    }

    public override void OnEnter()
    {
        player.animator.CrossFade("GroundedMovement", 0.1f);
    }

    public override void FixedUpdateStep()
    {
        if (player.wallrunningSurfaceContact != null && 
            Vector3.Dot(player.CameraRelativeInputDirection(), -player.wallrunningSurfaceContact.Value.normal) > 1e-4f)
        {
            playerSM.TransitionToState<GroundedToWallrunningState>();
            return;
        }

        /* Checks if player is on the ground. Consider doing a spherecast for more accuracy */
        if (player.IsGrounded())
        {
            Vector3 vDesired = player.groundMovementSpeed * player.CameraRelativeInputDirection();
            Vector3 a = 10.0f * (vDesired - player.Velocity);
            a = Vector3.ClampMagnitude(a, 100.0f);
            player.AddForce(a, ForceMode.Acceleration);

            Vector3 movementDir = player.CameraRelativeInputDirection();
            if(movementDir.magnitude > 1e-4f)
            {
                player.RotateCharacterToFace(movementDir, Vector3.up);
            }
        }
        else
        {
            playerSM.TransitionToState<AerialState>();
        }
    }

    public override void UpdateStep()
    {
        /* This belongs here because FixedUpdate would sometimes miss the spacebarDown event */
        if (player.input.spacebarDown)
        {
            playerSM.TransitionToState<JumpingState>();
            return;
        }
        float groundSpeed = Vector3.ProjectOnPlane(player.Velocity, Vector3.up).magnitude;
        player.animator.SetFloat("GroundSpeed", groundSpeed / player.groundMovementSpeed);
    }

    public override void OnExit()
    {

    }
}
