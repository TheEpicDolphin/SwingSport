using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpingState : PlayerState
{
    float jumpDuration = 0.1f;
    float t;
    float spacebarTime;

    public override void OnEnter()
    {
        this.t = 0.0f;
        this.spacebarTime = 0.0f;
    }

    public override void FixedUpdateStep()
    {
        player.AddForce(20.0f * Vector3.up, ForceMode.Acceleration);
    }

    public override void UpdateStep()
    {
        if (t >= jumpDuration)
        {
            float jumpPower = Mathf.Lerp(5.0f, 15.0f, spacebarTime / jumpDuration);
            player.AddForce(jumpPower * Vector3.up, ForceMode.VelocityChange);
            playerSM.TransitionToState<AerialState>();
            return;
        }
        if (player.input.spacebar)
        {
            /* If player is holding down spacebar while jumping, he/she will jump higher */
            spacebarTime += Time.deltaTime;
        }
        t += Time.deltaTime;
    }

    public override void OnExit()
    {

    }
}
