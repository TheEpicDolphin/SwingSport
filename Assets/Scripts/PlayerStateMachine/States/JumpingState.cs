using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpingState : PlayerState
{
    float minJumpDuration;

    public JumpingState(float minJumpDuration)
    {
        this.minJumpDuration = minJumpDuration;
    }

    public override void OnEnter()
    {
        throw new System.NotImplementedException();
    }

    public override PlayerState FixedUpdateStep(Player player)
    {
        /* If player is holding down spacebar while jumping, he/she will jump higher */
        if (player.input.spacebar)
        {
            player.activeRagdoll.AddAcceleration(100.0f * Vector3.up);
        }
        player.activeRagdoll.MatchRotation(player.playerCamera.transform.rotation);
        return this;
    }

    public override PlayerState UpdateStep(Player player)
    {
        if (minJumpDuration < 0)
        {
            return new AerialState();
        }
        minJumpDuration -= Time.deltaTime;
        return this;
    }
}
