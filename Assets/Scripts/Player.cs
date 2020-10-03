using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public ActiveRagdoll activeRagdoll;

    /* Camera that is following player */
    public PlayerCamera playerCamera;

    /* how fast player can move */
    public float groundMovementSpeed = 12.0f;

    public float airMovementSpeed = 15.0f;

    public Image cursorImage;

    public Transform handR;

    public Transform handL;

    public PlayerInputManager input;

    PlayerState currentState;

    // Start is called before the first frame update
    void Start()
    {
        input = gameObject.AddComponent<PlayerInputManager>();
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
        currentState = currentState.UpdateStep(this);
        playerCamera.UpdateCameraTargetRotation(input.mouseXDelta, input.mouseYDelta);
    }

    private void FixedUpdate()
    {
        currentState = currentState.FixedUpdateStep(this);
    }

    public Vector3 CameraRelativeInputDirection()
    {
        float moveHorizontal = input.horizontal;
        float moveVertical = input.vertical;
        Vector3 movement = moveVertical * Vector3.ProjectOnPlane(playerCamera.transform.forward, Vector3.up).normalized +
                            moveHorizontal * playerCamera.transform.right;
        return movement.normalized;
    }
}
