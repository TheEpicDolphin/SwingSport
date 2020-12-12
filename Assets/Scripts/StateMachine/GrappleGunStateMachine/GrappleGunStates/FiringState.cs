using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiringState : GrappleGunState
{
    float firingTime;
    const float maxFiringTime = 2.0f;
    public FiringState(GrappleGun grappleGun) : base(grappleGun)
    {
        
    }

    public override void OnEnter()
    {
        grappleGun.hookDidHitColliderAnnouncer.AddListener(HookDidHitCollider);
        firingTime = 0.0f;
        grappleGun.ShootHook();
        //grapplingGun.animator.CrossFade("Firing", 0.1f);
        grappleGun.fakeRopeRenderer.enabled = true;
    }

    public override void FixedUpdateStep()
    {
        
    }

    public override void UpdateStep()
    {
        firingTime += Time.deltaTime;
        if(firingTime > maxFiringTime)
        {
            grappleGun.stateMachine.TransitionToState(new RetractingHookState(grappleGun));
        }
    }

    public override void OnExit()
    {
        grappleGun.hookDidHitColliderAnnouncer.RemoveListener(HookDidHitCollider);
        grappleGun.fakeRopeRenderer.enabled = false;
    }

    private void HookDidHitCollider(Collider collider)
    {
        grappleGun.stateMachine.TransitionToState(new HookedState(grappleGun, collider));
    }
}
