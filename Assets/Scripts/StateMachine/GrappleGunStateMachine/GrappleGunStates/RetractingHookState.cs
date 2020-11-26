using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RetractingHookState : GrappleGunState
{
    public RetractingHookState(GrappleGunStateMachine grappleGunSM, GrappleGun grappleGun) : base(grappleGunSM, grappleGun)
    {

    }

    public override void OnEnter()
    {
        grappleGun.hook.isKinematic = true;
        //grapplingGun.animator.CrossFade("Retracting", 0.1f);
        grappleGun.fakeRopeRenderer.enabled = true;
    }

    public override void FixedUpdateStep()
    {

    }

    public override void UpdateStep()
    {
        if ()
        {
            grappleGunSM.TransitionToState<LoadedState>();
            return;
        }
    }

    public override void OnExit()
    {
        grappleGun.fakeRopeRenderer.enabled = false;
    }
}
