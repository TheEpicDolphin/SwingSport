using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunningState : PlayerState
{
    float maxWallrunningTime = 4.0f;
    float wallrunningTime;
    public override void OnEnter()
    {
        wallrunningTime = 0.0f;
        player.animator.CrossFade("Wallrunning", 0.1f);
    }

    public override void FixedUpdateStep()
    {
        /* Checks if player is touching ground */
        bool willLand = Physics.Raycast(player.AnimatedRigHipPosition(), Vector3.down, 1.6f, ~LayerMask.GetMask("Player"));
        if (willLand)
        {
            //return new LandingState(player, 0.15f);
            playerSM.TransitionToState<GroundedState>();
            return;
        }

        if(player.bumper.contactPoints.Count == 0)
        {
            playerSM.TransitionToState<AerialState>();
            return;
        }

        Vector3 movementDir = player.CameraRelativeInputDirection();
        foreach(ContactPoint contact in player.bumper.contactPoints)
        {
            Vector3 wallNormal = contact.normal;
            float dot = Vector3.Dot(movementDir, -wallNormal);
            if (dot < 0)
            {
                Vector3 upMovementForce = 10.0f * dot * Vector3.up;
                Vector3 planarMovementForce = Vector3.ProjectOnPlane(movementDir, wallNormal) * 10.0f;
                player.AddForce(planarMovementForce + upMovementForce, ForceMode.Acceleration);
                break;
            }
        }
    }

    public override void UpdateStep()
    {
        if (wallrunningTime < maxWallrunningTime)
        {
            wallrunningTime += Time.deltaTime;
        }
        else
        {
            //TODO: Maybe animate player sliding down wall?
        }
    }

    public override void OnExit()
    {
        
    }
}
