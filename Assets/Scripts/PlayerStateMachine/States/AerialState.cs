using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AerialState : PlayerState
{
    public override void OnEnter()
    {
        throw new System.NotImplementedException();
    }

    public override PlayerState FixedUpdateStep(Player player)
    {
        /* Checks if player is touching ground */
        bool isGrounded = Physics.Raycast(player.transform.position, Vector3.down, 1.3f, ~LayerMask.GetMask("Player"));
        if (isGrounded)
        {
            return new GroundedState();
        }

        /* Player is in the air. Allow jetpack-like movement */
        if (player.input.spacebar)
        {
            /* Propels player upwards */
            player.activeRagdoll.AddAcceleration(-1.1f * Physics.gravity);
        }
        /* Propels player left, right, forwards, and backwards */
        player.activeRagdoll.AddAcceleration(10.0f * player.CameraRelativeInputDirection());
        player.activeRagdoll.ApplyAirDrag();
        return this;
    }

    public override PlayerState UpdateStep(Player player)
    {
        return this;
    }
}
