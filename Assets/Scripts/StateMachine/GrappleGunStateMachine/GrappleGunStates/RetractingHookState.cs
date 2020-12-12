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
        grappleGun.hookDidFinishRetractingAnnouncer.AddListener(HookDidFinishRetracting);
        grappleGun.RetractHook();
        //grapplingGun.animator.CrossFade("Retracting", 0.1f);
        grappleGun.fakeRopeRenderer.enabled = true;
    }

    public override void FixedUpdateStep()
    {

    }

    public override void UpdateStep()
    {
        
    }

    public override void OnExit()
    {
        grappleGun.hookDidFinishRetractingAnnouncer.RemoveListener(HookDidFinishRetracting);
        grappleGun.fakeRopeRenderer.enabled = false;
    }

    private void HookDidFinishRetracting()
    {
        grappleGun.stateMachine.TransitionToState(new LoadedState(grappleGun));
    }
}
