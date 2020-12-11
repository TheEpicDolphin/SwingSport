using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGrappleGunWielder
{
    Ray AimingLineOfSightRay();
}

public class GrappleGun : MonoBehaviour, IGrapplingHookLauncher
{
    public float maxRopeLength = 150.0f;

    private float hookLaunchForce = 200.0f;

    float hookZoomRateMultiplier = 20.0f;

    public HookGunCursor cursor;

    public LineRenderer fakeRopeRenderer;

    public Rope rope;

    public PlayerInputManager input;

    public GrapplingHook hook;

    private GrappleGunStateMachine grappleGunSM;

    Vector3 focusPoint;

    public IGrappleGunWielder wielder;

    private void Awake()
    {
        grappleGunSM = new GrappleGunStateMachine(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        grappleGunSM.InitWithState(new LoadedState(grappleGunSM, this));
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

    public void ShootHook()
    {
        Vector3 aimingTarget = HitTest();
        Vector3 shootDirection = (aimingTarget - hook.transform.position).normalized;
        hook.Launch(hookLaunchForce * shootDirection);
    }

    private Vector3 HitTest()
    {
        Vector3 aimingTarget;
        Ray losRay = wielder.AimingLineOfSightRay();
        
        // Prevent raycast from hitting something in front of camera but behind gun
        float startT = Vector3.Dot(transform.position - losRay.origin, losRay.direction);
        RaycastHit hit;
        LayerMask obstacleLayer = LayerMask.NameToLayer("Obstacle");
        LayerMask ballLayer = LayerMask.NameToLayer("Ball");
        if (Physics.Raycast(new Ray(losRay.GetPoint(startT), losRay.direction), out hit, maxRopeLength, obstacleLayer | ballLayer))
        {
            aimingTarget = hit.point;
        }
        else
        {
            aimingTarget = losRay.GetPoint(maxRopeLength);
        }
        return aimingTarget;
    }

    public void AttachHookToTransform(Transform trans)
    {
        hook.AttachToTransformAtPositionWithOrientation(trans, hook.transform.position, hook.transform.rotation);
    }

    void IGrapplingHookLauncher.DidHitCollider(Collider collider)
    {
        grappleGunSM.HookDidAttachEvent.Announce(collider);
    }
}
