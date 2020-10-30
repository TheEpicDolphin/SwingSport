using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AerialState : PlayerState
{
    public override void OnEnter()
    {
        player.animator.CrossFade("Falling", 0.1f);
    }

    public override PlayerState FixedUpdateStep(Player player)
    {
        /* Checks if player is touching ground */
        bool willLand = Physics.Raycast(player.AnimatedRigHipPosition(), Vector3.down, 1.6f, ~LayerMask.GetMask("Player"));
        if (willLand)
        {
            return new GroundedState(player);
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
        return this;
    }

    public override PlayerState UpdateStep(Player player)
    {
        return this;
    }
}
