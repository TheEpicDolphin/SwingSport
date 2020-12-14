﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundedToWallrunningState : PlayerState
{
    float maxJumpDuration = 0.1f;
    float t;

    public GroundedToWallrunningState(Player player) : base(player)
    {

    }

    public override void OnEnter()
    {
        this.t = 0.0f;
        player.animator.CrossFade("Jump", 0.1f);
        player.AddForce(10.0f * Vector3.up, ForceMode.VelocityChange);
    }

    public override void FixedUpdateStep()
    {
        if (!player.IsGrounded())
        {
            player.stateMachine.TransitionToState(new GroundedState(player));
        }
    }

    public override void UpdateStep()
    {
        if (t >= maxJumpDuration)
        {
            Debug.Log("FAILED");
            player.stateMachine.TransitionToState(new AerialState(player));
            return;
        }
        t += Time.deltaTime;
    }

    public override void OnExit()
    {

    }
}