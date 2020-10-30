using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunningState : PlayerState
{
    float maxWallrunningTime = 4.0f;
    float wallrunningTime = 0.0f;
    public WallRunningState(Player player)
    {
        player.animator.CrossFade("Wallrunning", 0.1f);
    }

    
    public WallRunningState()
    {

    }

    public override void OnEnter(Player player)
    {
        
        
    }

    public override void OnEnter()
    {
        wallrunningTime = 0.0f;
        player.animator.CrossFade("Wallrunning", 0.1f);
    }

    public override PlayerState FixedUpdateStep(Player player)
    {
        /* Checks if player is touching ground */
        bool willLand = Physics.Raycast(player.AnimatedRigHipPosition(), Vector3.down, 1.6f, ~LayerMask.GetMask("Player"));
        if (willLand)
        {
            //return new LandingState(player, 0.15f);
            return new GroundedState(player);
        }

        if(player.bumper.contactPoints.Count == 0)
        {
            return new AerialState(player);
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
        return this;
    }

    public override PlayerState UpdateStep(Player player)
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
}
