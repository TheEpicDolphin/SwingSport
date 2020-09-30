using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovementController : MonoBehaviour
{
    ActiveRagdoll activeRagdoll;

    /* Transform of camera following player */
    public Transform cameraTrans;

    /* how fast player can move */
    float groundMovementSpeed = 12.0f;

    float airMovementSpeed = 15.0f;

    /* movement vector of the player*/
    Vector3 movement = Vector3.zero;

    /* determines whether the player is touching the ground */
    public bool isGrounded = false;

    public Image cursorImage;

    public Transform handR;

    public Transform handL;

    // Start is called before the first frame update
    void Start()
    {
        activeRagdoll = GetComponent<ActiveRagdoll>();

        if (handR)
        {
            GameObject hookGunGO = (GameObject)Instantiate(Resources.Load("Prefabs/HookGun"));
            HookGun hookGun = hookGunGO.GetComponent<HookGun>();
            hookGun.Equip(handR, handR.position + 0.25f * handR.transform.forward,
                            handR.rotation, false);
            hookGun.setControls(1);
            hookGun.setColor(Color.red);
            hookGun.cursor.cursorImage = cursorImage;
        }

        if (handL)
        {
            GameObject magnetoGloveGO = new GameObject();
            MagnetoGlove magnetoGlove = magnetoGloveGO.AddComponent<MagnetoGlove>();
            magnetoGlove.Equip(handL, handL.position, handL.rotation, false);
            /*
            GameObject ballHookGunGO = (GameObject)Instantiate(Resources.Load("Prefabs/HookGun"));
            HookGun ballHookGun = ballHookGunGO.GetComponent<HookGun>();
            ballHookGun.AttachTo(handL, handL.position + 0.25f * handL.transform.forward,
                            handL.rotation, false);
            ballHookGun.setControls(0);
            ballHookGun.setColor(Color.blue);
            ballHookGun.cursor.cursorImage = cursorImage;
            */
        }
        
    }

    private void Update()
    {
        /* This belongs here because FixedUpdate would sometimes miss the spacebarDown event */
        if (isGrounded && PlayerInputManager.Instance.spacebarDown)
        {
            /* jump */
            activeRagdoll.AddVelocityChange(10.0f * Vector3.up);
        }
    }

    private void FixedUpdate()
    {
        /* Checks if player is on the ground. Consider doing a spherecast for more accuracy */
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.3f, ~LayerMask.GetMask("Player"));

        /* Handles movement based on camera direction */
        float moveHorizontal = PlayerInputManager.Instance.horizontal;
        float moveVertical = PlayerInputManager.Instance.vertical;
        movement = moveVertical * Vector3.ProjectOnPlane(cameraTrans.forward, Vector3.up).normalized +
                            moveHorizontal * cameraTrans.right;

        if (isGrounded)
        {
            /* Player is on the ground */
            Vector3 vDesired = groundMovementSpeed * movement.normalized;
            float k = (1 / Time.fixedDeltaTime) * 0.4f;
            Vector3 a = k * (vDesired - activeRagdoll.Velocity);
            activeRagdoll.AddAcceleration(a);
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

            activeRagdoll.ApplyAirDrag();
        }
        /* Rotating character is done in RagdollAnimController */
        //Vector3 turningTorque = 100.0f * Vector3.Cross(transform.forward, cameraTrans.forward);
        //rb.AddTorque(turningTorque, ForceMode.Acceleration);

        /* Rotates player to face in direction of camera */
        activeRagdoll.MatchRotation(Camera.main.transform.rotation);

    }
}
