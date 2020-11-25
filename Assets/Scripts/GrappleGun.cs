using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleGun : MonoBehaviour
{
    public float maxRopeLength = 150.0f;

    public float hookLaunchForce = 200.0f;

    float hookZoomRateMultiplier = 20.0f;

    public HookGunCursor cursor;

    public LineRenderer fakeRopeRenderer;

    public Rope rope;

    public PlayerInputManager input;

    public GrapplingHook hook;

    private GrappleGunStateMachine grappleGunSM;

    Vector3 focusPoint;

    private void Awake()
    {
        grappleGunSM = new GrappleGunStateMachine(this, new List<System.Type>(){
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
        grappleGunSM.InitWithState<LoadedState>();
    }

    // Update is called once per frame
    void Update()
    {
        grappleGunSM.UpdateStep();
    }

    private void FixedUpdate()
    {
        grappleGunSM.FixedUpdateStep();
    }

    
}
