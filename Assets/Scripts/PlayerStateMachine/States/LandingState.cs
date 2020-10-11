using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandingState : PlayerState
{
    float landDuration;
    float t;
    bool landed;

    public LandingState(Player player, float landDuration)
    {
        this.landDuration = landDuration;
        this.t = 0.0f;
        this.landed = false;
        player.animator.CrossFade("GroundedMovement", 0.1f);
    }

    public override void OnEnter()
    {
        throw new System.NotImplementedException();
    }

    public override PlayerState FixedUpdateStep(Player player)
    {
        /* Checks if player is touching ground */
        bool landed = Physics.Raycast(player.AnimatedRigHipPosition(), Vector3.down, 1.3f, ~LayerMask.GetMask("Player"));
        if (!landed)
        {
            /* TODO: use PID controller to slow player down to reasonable speed for landing */
            player.AddForce(25.0f * Vector3.up, ForceMode.Acceleration);
            t = 0.0f;
        }
        return this;
    }

    public override PlayerState UpdateStep(Player player)
    {
        if (t >= landDuration)
        {
            return new GroundedState(player);
        }
        t += Time.deltaTime;
        return this;
    }
}
