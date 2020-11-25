﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadedState : GrappleGunState
{
    public LoadedState(GrappleGunStateMachine grapplingGunSM, GrappleGun grappleGun) : base(grapplingGunSM, grappleGun)
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
            grappleGunSM.TransitionToState<FiringState>();
        }
    }

    public override void OnExit()
    {

    }
}
