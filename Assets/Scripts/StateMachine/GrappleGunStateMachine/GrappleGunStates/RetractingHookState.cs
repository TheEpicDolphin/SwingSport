using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RetractingHookState : GrappleGunState
{
    public RetractingHookState(GrappleGun grappleGun) : base(grappleGun)
    {

    }

    public override void OnEnter()
    {
        grappleGun.RetractHook();
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
            grappleGun.stateMachine.TransitionToState(new LoadedState(grappleGun));
            return;
        }
    }

    public override void OnExit()
    {
        grappleGun.fakeRopeRenderer.enabled = false;
    }
}
