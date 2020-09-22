﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RagdollMovementController : MonoBehaviour
{
    ActiveRagdoll activeRagdoll;

    /* Transform of camera following player */
    public Transform cameraTrans;

    /* how fast player can move */
    float movementSpeed = 12.0f;

    /* movement vector of the player*/
    Vector3 movement = Vector3.zero;

    /* determines whether the player is touching the ground */
    public bool isGrounded = false;

    /* if true, player is rocketing up */
    public bool rocketUp = false;

    ConfigurableJoint confJoint;

    public Image cursorImage;

    public Transform handR;

    public Transform handL;

    // Start is called before the first frame update
    void Start()
    {
        activeRagdoll = GetComponent<ActiveRagdoll>();
        confJoint = GetComponent<ConfigurableJoint>();

        if (handR)
        {
            //Instantiate hook gun in player's hand
            GameObject hookGunGO = (GameObject)Instantiate(Resources.Load("Prefabs/HookGun"),
                handR.position + 0.15f * handR.transform.forward, handR.rotation, handR);
            HookGun hookGun = hookGunGO.GetComponent<HookGun>();
            //hookGun.camWobbleDelegate = mainCamera.GetComponent<CameraController>().AddWobble;
            hookGun.cursor.cursorImage = cursorImage;
        }

        if (handL)
        {
            handL.gameObject.AddComponent<MagnetoGlove>();
        }
        
    }

    private void FixedUpdate()
    {
        /* Checks if player is on the ground. Consider doing a spherecast for more accuracy */
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.3f, ~(1 << 9));

        /* Handles movement based on camera direction */
        float moveHorizontal = PlayerInputManager.Instance.horizontal;
        float moveVertical = PlayerInputManager.Instance.vertical;
        movement = moveVertical * Vector3.ProjectOnPlane(cameraTrans.forward, Vector3.up).normalized +
                            moveHorizontal * cameraTrans.right;

        if (isGrounded)
        {
            /* Player is on the ground */
            Vector3 vDesired = movementSpeed * movement.normalized;
            float k = (1 / Time.fixedDeltaTime) * 0.4f;
            Vector3 a = k * (vDesired - activeRagdoll.Velocity);
            activeRagdoll.AddAcceleration(a);

            if (PlayerInputManager.Instance.spacebarDown)
            {
                /* jump */
                activeRagdoll.AddVelocityChange(10.0f * Vector3.up);
            }
        }
        else
        {
            /* Player is in the air. Allow jetpack-like movement */
            if (PlayerInputManager.Instance.spacebar)
            {
                /* Propels player upwards */
                activeRagdoll.AddAcceleration(-1.1f * Physics.gravity);
            }
            /* Propels player left, right, forwards, and backwards */
            activeRagdoll.AddAcceleration(10.0f * movement.normalized);
        }
        /* Rotating character is done in RagdollAnimController */
        //Vector3 turningTorque = 100.0f * Vector3.Cross(transform.forward, cameraTrans.forward);
        //rb.AddTorque(turningTorque, ForceMode.Acceleration);

        /* Rotates player to face in direction of camera */
        activeRagdoll.MatchRotation(Camera.main.transform.rotation);
    }
}
