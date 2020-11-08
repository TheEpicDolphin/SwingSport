using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpingState : PlayerState
{
    float maxJumpDuration = 0.15f;
    float jumpEventTime = 0.1f;
    float elapsedTime;
    float spacebarTime;
    bool hasJumped;

    public JumpingState(PlayerStateMachine playerSM, Player player) : base(playerSM, player)
    {

    }

    public override void OnEnter()
    {
        this.elapsedTime = 0.0f;
        this.spacebarTime = 0.0f;
        hasJumped = false;
        player.animator.CrossFade("Jump", 0.1f);
    }

    public override void FixedUpdateStep()
    {
        if (elapsedTime >= jumpEventTime && !hasJumped)
        {
            float jumpPower = Mathf.Lerp(5.0f, 15.0f, spacebarTime / elapsedTime);
            player.AddForce(jumpPower * Vector3.up, ForceMode.VelocityChange);
            hasJumped = true;
        }

        if (!player.IsGrounded())
        {
            playerSM.TransitionToState<AerialState>();
            return;
        }
    }

    public override void UpdateStep()
    {
        if (elapsedTime >= maxJumpDuration)
        {
            playerSM.TransitionToState<GroundedState>();
            return;
        }
        if (player.input.spacebar)
        {
            /* If player is holding down spacebar while jumping, he/she will jump higher */
            spacebarTime += Time.deltaTime;
        }
        elapsedTime += Time.deltaTime;
    }

    public override void OnExit()
    {

    }
}
