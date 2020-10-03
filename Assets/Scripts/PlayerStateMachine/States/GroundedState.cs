using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundedState : PlayerState
{
    public override void OnEnter()
    {
        throw new System.NotImplementedException();
    }

    public override PlayerState FixedUpdateStep(Player player)
    {
        /* Checks if player is on the ground. Consider doing a spherecast for more accuracy */
        bool isGrounded = Physics.Raycast(player.transform.position, Vector3.down, 1.3f, ~LayerMask.GetMask("Player"));

        if (!isGrounded)
        {
            return new AerialState();
        }

        Vector3 vDesired = player.groundMovementSpeed * player.CameraRelativeInputDirection();
        float k = (1 / Time.fixedDeltaTime) * 0.4f;
        Vector3 a = k * (vDesired - player.activeRagdoll.Velocity);
        player.activeRagdoll.AddAcceleration(a);


        /* Rotating character is done in RagdollAnimController */
        //Vector3 turningTorque = 100.0f * Vector3.Cross(transform.forward, cameraTrans.forward);
        //rb.AddTorque(turningTorque, ForceMode.Acceleration);

        /* Rotates player to face in direction of camera */
        player.activeRagdoll.MatchRotation(player.playerCamera.transform.rotation);
        return this;
    }

    public override PlayerState UpdateStep(Player player)
    {
        /* This belongs here because FixedUpdate would sometimes miss the spacebarDown event */
        if (player.input.spacebarDown)
        {
            /* jump */
            player.activeRagdoll.AddVelocityChange(10.0f * Vector3.up);

            // TODO: Delay for a bit to allow for player to take off ground
            return new JumpingState(0.25f);
        }
        return this;
    }

    

    
}
