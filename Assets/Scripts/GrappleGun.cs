using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleGun : MonoBehaviour
{
    const float maxRopeLength = 150.0f;

    float hookLaunchForce = 200.0f;

    float hookZoomRateMultiplier = 20.0f;

    public HookGunCursor cursor;

    private LineRenderer fakeRopeRenderer;

    private Rope rope;

    private GrappleGunStateMachine grappleGunSM;

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
        grappleGunSM.InitWithState<GroundedState>();
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
