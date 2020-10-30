using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpingState : PlayerState
{
    float jumpDuration;
    float t;
    float spacebarTime;

    public JumpingState(float jumpDuration)
    {
        
        this.jumpDuration = jumpDuration;
        this.t = 0.0f;
        this.spacebarTime = 0.0f;
    }

    public override void OnEnter()
    {
        throw new System.NotImplementedException();
    }

    public override PlayerState FixedUpdateStep(Player player)
    {
        player.AddForce(20.0f * Vector3.up, ForceMode.Acceleration);
        return this;
    }

    public override PlayerState UpdateStep(Player player)
    {
        if (t >= jumpDuration)
        {
            float jumpPower = Mathf.Lerp(5.0f, 15.0f, spacebarTime / jumpDuration);
            player.AddForce(jumpPower * Vector3.up, ForceMode.VelocityChange);
            return new AerialState(player);
        }
        if (player.input.spacebar)
        {
            /* If player is holding down spacebar while jumping, he/she will jump higher */
            spacebarTime += Time.deltaTime;
        }
        t += Time.deltaTime;
        return this;
    }
}
