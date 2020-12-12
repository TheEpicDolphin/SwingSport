using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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

    public MonoBehaviourStateMachine stateMachine;

    Vector3 focusPoint;

    public IGrappleGunWielder wielder;

    public UnityEvent<Collider> hookDidHitColliderAnnouncer;

    public UnityEvent hookDidFinishRetractingAnnouncer;

    private void Awake()
    {
        hookDidHitColliderAnnouncer = new UnityEvent<Collider>();
        hookDidFinishRetractingAnnouncer = new UnityEvent();
        stateMachine = new MonoBehaviourStateMachine();
    }

    // Start is called before the first frame update
    void Start()
    {
        stateMachine.InitWithState(new LoadedState(this));
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.UpdateStep();
    }

    private void FixedUpdate()
    {
        stateMachine.FixedUpdateStep();
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

    public void RetractHook()
    {
        hook.Retract();
    }

    public void AttachHookToTransform(Transform trans)
    {
        hook.Attach(trans);
    }

    void IGrapplingHookLauncher.DidHitCollider(Collider collider)
    {
        hookDidHitColliderAnnouncer.Invoke(collider);
    }

    void IGrapplingHookLauncher.DidFinishRetracting()
    {
        hookDidFinishRetractingAnnouncer.Invoke();
    }
}
