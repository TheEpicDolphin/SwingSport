using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AerialState : PlayerState
{
    public AerialState(Player player)
    {
        player.activeRagdoll.animator.CrossFade("Falling", 0.1f);
    }

    public override void OnEnter()
    {
        throw new System.NotImplementedException();
    }

    public override PlayerState FixedUpdateStep(Player player)
    {
        /* Checks if player is touching ground */
        bool willLand = Physics.Raycast(player.transform.position, Vector3.down, 1.6f, ~LayerMask.GetMask("Player"));
        if (willLand)
        {
            //return new LandingState(player, 0.15f);
            return new GroundedState(player);
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

        //player.activeRagdoll.MatchRotation(player.playerCamera.transform.rotation);
        player.activeRagdoll.MatchRotation(Quaternion.LookRotation(player.CameraRelativeInputDirection(), Vector3.up));
        return this;
    }

    public override PlayerState UpdateStep(Player player)
    {
        return this;
    }
}
