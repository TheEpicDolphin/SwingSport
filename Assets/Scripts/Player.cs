using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public ActiveRagdoll activeRagdoll;

    /* Camera that is following player */
    public PlayerCamera followingCamera;

    /* how fast player can move */
    public float groundMovementSpeed = 12.0f;

    public float airMovementSpeed = 15.0f;

    public Image cursorImage;

    public Transform ragdollHandR;

    public Transform ragdollHandL;

    public PlayerInputManager input;

    PlayerState currentState;

    CapsuleCollider bumper;

    public Animator animator;

    private Rigidbody playerRb;

    float airDrag = 0.5f;

    float maxSpeed = 60.0f;

    private Transform animatedRigHip;

    Quaternion characterRotation = Quaternion.identity;

    public Vector3 Velocity
    {
        get
        {
            return playerRb.velocity;
        }
    }

    private void Awake()
    {
        input = gameObject.AddComponent<PlayerInputManager>();

        bumper = GetComponent<CapsuleCollider>();
        gameObject.layer = LayerMask.NameToLayer("Bumper");
        animator = GetComponent<Animator>();
        playerRb = GetComponent<Rigidbody>();

        animatedRigHip = transform.GetChild(0);
        activeRagdoll.CreateActiveRagdoll(animatedRigHip, playerRb.mass);

        if (ragdollHandR)
        {
            GameObject hookGunGO = (GameObject)Instantiate(Resources.Load("Prefabs/HookGun"));
            HookGun hookGun = hookGunGO.GetComponent<HookGun>();
            hookGun.Equip(ragdollHandR, ragdollHandR.position + 0.25f * ragdollHandR.transform.forward,
                            ragdollHandR.rotation);
            hookGun.setControls(1);
            hookGun.setColor(Color.red);
            hookGun.cursor.cursorImage = cursorImage;
        }

        if (ragdollHandL)
        {
            //GameObject magnetoGloveGO = new GameObject();
            //MagnetoGlove magnetoGlove = magnetoGloveGO.AddComponent<MagnetoGlove>();
            //magnetoGlove.Equip(ragdollHandL, ragdollHandL.position, ragdollHandL.rotation, false);
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

    // Start is called before the first frame update
    void Start()
    {
        currentState = new GroundedState(this);
    }

    private void Update()
    {
        currentState = currentState.UpdateStep(this);
        followingCamera.UpdateCameraTargetRotation(input.mouseXDelta, input.mouseYDelta);

        /*
        MagnetoGlove magnetoGlove = ragdollHandL.GetComponent<MagnetoGlove>();
        if (magnetoGlove)
        {
            magnetoGlove.Handle(this);
        }
        */

        DrawAnimatedRig(animatedRigHip);
    }

    private void OnAnimatorMove()
    {
        characterRotation *= animator.deltaRotation;
    }

    private void LateUpdate()
    {
        /* It is important that we set the hip rotation here before we do any IK */
        animatedRigHip.localRotation = characterRotation;

        // TODO: Perform IK below

    }

    private void FixedUpdate()
    {
        currentState = currentState.FixedUpdateStep(this);
    }

    public Vector3 CameraRelativeInputDirection()
    {
        float moveHorizontal = input.horizontal;
        float moveVertical = input.vertical;
        Vector3 movement = moveVertical * Vector3.ProjectOnPlane(followingCamera.transform.forward, Vector3.up).normalized +
                            moveHorizontal * followingCamera.transform.right;
        return movement.normalized;
    }

    public void AddForce(Vector3 force, ForceMode mode)
    {
        playerRb.AddForce(force, mode);
    }

    public void RotateCharacterToFace(Vector3 forward, Vector3 upwards)
    {
        characterRotation = Quaternion.LookRotation(forward, upwards);
    }

    public void ApplyAirDrag()
    {
        Vector3 curVel = playerRb.velocity;
        float curSpeed = curVel.magnitude;
        playerRb.AddForce(-airDrag * Mathf.Max(0.0f, curSpeed - maxSpeed) * curVel);
    }

    public Vector3 AnimatedRigHipPosition()
    {
        return animatedRigHip.position;
    }

    public float AnimatedHipTargetY()
    {
        return animatedRigHip.position.y;
    }

    public void DrawAnimatedRig(Transform animBone)
    {
        for (int i = 0; i < animBone.childCount; i++)
        {
            Transform childAnimBone = animBone.GetChild(i);
            Debug.DrawLine(animBone.transform.position, childAnimBone.transform.position, Color.red, 0.0f, false);
            DrawAnimatedRig(childAnimBone);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        //Debug.Log("STAY");
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("ENTERED");
    }

    private void OnCollisionExit(Collision collision)
    {
        //Debug.Log("EXIT");
    }
}
