using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiringState : GrappleGunState
{
    float firingTime;
    const float maxFiringTime = 2.0f;
    public FiringState(GrappleGunStateMachine grappleGunSM, GrappleGun grappleGun) : base(grappleGunSM, grappleGun)
    {
        
    }

    public override void OnEnter()
    {
        grappleGunSM.hookDidAttachEvent.AddListener(this);
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
            grappleGunSM.TransitionToState(new RetractingHookState(grappleGunSM, grappleGun));
        }
    }

    public override void OnExit()
    {
        grappleGunSM.hookDidAttachEvent.RemoveListener(this);
        grappleGun.fakeRopeRenderer.enabled = false;
    }
}
