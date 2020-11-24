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

    public ContactPoint? wallrunningSurfaceContact;

    public Animator animator;

    private Rigidbody playerRb;

    private Transform animatedRigHip;

    private Quaternion characterRotation = Quaternion.identity;

    private Material animatedBonesMat;

    private PlayerStateMachine playerSM;

    const float airDrag = 0.5f;

    const float maxSpeed = 60.0f;

    public Vector3 Velocity
    {
        get
        {
            return playerRb.velocity;
        }
    }

    private void Awake()
    {
        wallrunningSurfaceContact = null;

        animatedBonesMat = new Material(Shader.Find("Unlit/Color"));
        animatedBonesMat.color = Color.cyan;
        input = gameObject.AddComponent<PlayerInputManager>();

        animator = GetComponentInChildren<Animator>();
        playerRb = GetComponent<Rigidbody>();

        animatedRigHip = transform.GetChild(0).GetChild(0);
        activeRagdoll.CreateActiveRagdoll(animatedRigHip, playerRb.mass);

        if (ragdollHandR)
        {
            /*
            GameObject hookGunGO = (GameObject)Instantiate(Resources.Load("Prefabs/HookGun"));
            HookGun hookGun = hookGunGO.GetComponent<HookGun>();
            hookGun.Equip(ragdollHandR, ragdollHandR.position + 0.25f * ragdollHandR.transform.forward,
                            ragdollHandR.rotation);
            hookGun.setControls(1);
            hookGun.setColor(Color.red);
            hookGun.cursor.cursorImage = cursorImage;
            */
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

        
        playerSM = new PlayerStateMachine(this, new List<System.Type>(){
            typeof(GroundedState),
            typeof(JumpingState),
            typeof(AerialState),
            typeof(WallRunningState),
            typeof(GroundedToWallrunningState),
        });
        
    }

    // Start is called before the first frame update
    void Start()
    {
        playerSM.InitWithState<GroundedState>();
    }

    private void Update()
    {
        playerSM.UpdateStep();
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

    private void LateUpdate()
    {
        /* It is important that we set the hip rotation here before we do any IK */
        Quaternion animationOffset = animatedRigHip.localRotation;
        animatedRigHip.localRotation = characterRotation * animationOffset;
        // TODO: Perform IK below

    }

    private void FixedUpdate()
    {
        playerSM.FixedUpdateStep();

        /* Clear wallrunning surface contacts from last OnCollisionStay */
        wallrunningSurfaceContact = null;
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
        //characterRotation = Quaternion.Slerp(animatedRigHip.localRotation, Quaternion.LookRotation(forward, upwards), 10.0f * Time.deltaTime);
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

    public Vector3 AnimatedRigHipForward()
    {
        return animatedRigHip.forward;
    }

    public bool IsGrounded()
    {
        /* Takes into account current player velocity to determine if player will be grounded 
           within the next physics frame */
        float raycastDistance = 1.25f + Mathf.Max(0.0f, -Velocity.y * Time.fixedDeltaTime); 
        return Physics.Raycast(AnimatedRigHipPosition(), Vector3.down, raycastDistance, ~LayerMask.GetMask("Player"));
    }

    private void OnCollisionStay(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            float normalAngle = Vector3.Angle(contact.normal, Vector3.up);
            float angleFromYPlane = Mathf.Abs(normalAngle - 90.0f);
            if (contact.thisCollider.tag == "Bumper" && angleFromYPlane < 30.0f)
            {
                wallrunningSurfaceContact = contact;
                //Debug.DrawRay(contact.point, contact.normal, Color.red);
            }
        }
    }

    public void Draw()
    {
        if (!animatedBonesMat)
        {
            Debug.LogError("Please Assign a material on the inspector");
            return;
        }
        GL.PushMatrix();
        animatedBonesMat.SetPass(0);
        //GL.LoadOrtho();

        GL.Begin(GL.LINES);
        GL.Color(Color.cyan);
        DrawAnimatedRig(animatedRigHip);
        GL.End();

        GL.PopMatrix();
    }

    private void DrawAnimatedRig(Transform animBone)
    {
        for (int i = 0; i < animBone.childCount; i++)
        {
            Transform childAnimBone = animBone.GetChild(i);
            GL.Vertex(animBone.transform.position);
            GL.Vertex(childAnimBone.transform.position);
            //Debug.DrawLine(animBone.transform.position, childAnimBone.transform.position, Color.red, 0.0f, false);
            DrawAnimatedRig(childAnimBone);
        }
    }

}
