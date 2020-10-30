using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundedState : PlayerState
{

    public override void OnEnter()
    {
        player.animator.CrossFade("GroundedMovement", 0.1f);
    }

    public override PlayerState FixedUpdateStep(Player player)
    {
        RaycastHit hit;
        /* Checks if player is on the ground. Consider doing a spherecast for more accuracy */
        if (Physics.Raycast(player.AnimatedRigHipPosition(), Vector3.down, out hit, 1.3f, ~LayerMask.GetMask("Player")))
        {
            Vector3 vDesired = player.groundMovementSpeed * player.CameraRelativeInputDirection();
            Vector3 a = 10.0f * (vDesired - player.Velocity);
            a = Vector3.ClampMagnitude(a, 100.0f);
            player.AddForce(a, ForceMode.Acceleration);


            Vector3 movementDir = player.CameraRelativeInputDirection();
            if(movementDir.magnitude > 1e-4f)
            {
                player.RotateCharacterToFace(movementDir, Vector3.up);
            }
            return this;
        }
        else
        {
            playerSM.TransitionToState();
            return new AerialState(player);
        }
    }

    public override PlayerState UpdateStep(Player player)
    {
        /* This belongs here because FixedUpdate would sometimes miss the spacebarDown event */
        if (player.input.spacebarDown)
        {
            playerSM.TransitionToState();
            return new JumpingState(0.1f);
        }
        float groundSpeed = Vector3.ProjectOnPlane(player.Velocity, Vector3.up).magnitude;
        player.animator.SetFloat("GroundSpeed", groundSpeed / player.groundMovementSpeed);
        return this;
    }

    

    
}
