﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundedToWallrunningState : PlayerState
{
    float maxJumpDuration = 0.1f;
    float t;

    public GroundedToWallrunningState(PlayerStateMachine playerSM, Player player) : base(playerSM, player)
    {

    }

    public override void OnEnter()
    {
        this.t = 0.0f;
        player.AddForce(5.0f * Vector3.up, ForceMode.VelocityChange);
    }

    public override void FixedUpdateStep()
    {
        bool onGround = Physics.Raycast(player.AnimatedRigHipPosition(), Vector3.down, 1.6f, ~LayerMask.GetMask("Player"));
        if (!onGround)
        {
            playerSM.TransitionToState<WallRunningState>();
        }
    }

    public override void UpdateStep()
    {
        if (t >= maxJumpDuration)
        {
            Debug.Log("FAILED");
            playerSM.TransitionToState<AerialState>();
            return;
        }
        t += Time.deltaTime;
    }

    public override void OnExit()
    {

    }
}
