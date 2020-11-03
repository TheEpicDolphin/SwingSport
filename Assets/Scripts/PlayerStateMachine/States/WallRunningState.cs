using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunningState : PlayerState
{
    float maxWallrunningTime = 1.5f;
    float wallrunningTime;

    public WallRunningState(PlayerStateMachine playerSM, Player player) : base(playerSM, player)
    {

    }

    public override void OnEnter()
    {
        wallrunningTime = 0.0f;
        player.animator.CrossFade("Wallrunning", 0.1f);
    }

    public override void FixedUpdateStep()
    {
        if (player.IsGrounded())
        {
            playerSM.TransitionToState<GroundedState>();
            return;
        }

        if(player.wallrunningSurfaceContact == null || wallrunningTime >= maxWallrunningTime)
        {
            playerSM.TransitionToState<AerialState>();
            return;
        }

        Vector3 movementDir = player.CameraRelativeInputDirection();
        Vector3 wallNormal = player.wallrunningSurfaceContact.Value.normal;
        float dot = Vector3.Dot(movementDir, -wallNormal);
        if (dot > 0)
        {
            Vector3 upMovementAcceleration = -2.0f * Physics.gravity.y * (1.0f - wallrunningTime / maxWallrunningTime) * Vector3.up;
            Vector3 vDesired = player.groundMovementSpeed * Vector3.ProjectOnPlane(movementDir, wallNormal);
            Vector3 a = 10.0f * (vDesired - Vector3.ProjectOnPlane(player.Velocity, wallNormal));
            Vector3 planarMovementAcceleration = Vector3.ClampMagnitude(a, 100.0f);
            player.AddForce(planarMovementAcceleration + upMovementAcceleration, ForceMode.Acceleration);

            /* Unity cross product is left-handed */
            Vector3 wallParallel = Vector3.Cross(wallNormal, Vector3.up).normalized;
            float wallrunningDot = Vector3.Dot(movementDir, wallParallel);
            player.animator.SetFloat("WallrunningOrientation", (wallrunningDot + 1) / 2);
            player.RotateCharacterToFace(-wallNormal, Vector3.up);
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
