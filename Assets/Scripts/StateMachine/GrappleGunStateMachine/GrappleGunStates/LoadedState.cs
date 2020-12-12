using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadedState : GrappleGunState
{
    public LoadedState(GrappleGun grappleGun) : base(grappleGun)
    {

    }

    public override void OnEnter()
    {
        //grapplingGun.animator.CrossFade("Loaded", 0.1f);
    }

    public override void FixedUpdateStep()
    {
        
    }

    public override void UpdateStep()
    {
        if (grappleGun.input.leftMouse)
        {
            grappleGun.stateMachine.TransitionToState(new FiringState(grappleGun));
        }
    }

    public override void OnExit()
    {

    }
}
