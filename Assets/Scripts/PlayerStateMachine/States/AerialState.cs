using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AerialState : PlayerState
{
    public AerialState(PlayerStateMachine playerSM, Player player) : base(playerSM, player)
    {

    }

    public override void OnEnter()
    {
        player.animator.CrossFade("Falling", 0.1f);
    }

    public override void FixedUpdateStep()
    {
        if (player.IsGrounded())
        {
            playerSM.TransitionToState<GroundedState>();
            return;
        }

        if (player.wallrunningSurfaceContact != null &&
            Vector3.Dot(player.CameraRelativeInputDirection(), -player.wallrunningSurfaceContact.Value.normal) > 1e-4f &&
            player.Velocity.y >= 0.0f)
        {
            playerSM.TransitionToState<WallRunningState>();
        }

        /* Player is in the air. Allow jetpack-like movement */
        if (player.input.spacebar)
        {
            /* Propels player upwards */
            //player.AddForce(-1.1f * Physics.gravity, ForceMode.Acceleration);
        }
        /* Propels player left, right, forwards, and backwards */
        player.AddForce(10.0f * player.CameraRelativeInputDirection(), ForceMode.Acceleration);
        player.ApplyAirDrag();

        player.RotateCharacterToFace(player.followingCamera.transform.forward, Vector3.up);
    }

    public override void UpdateStep()
    {
        
    }

    public override void OnExit()
    {

    }
}
