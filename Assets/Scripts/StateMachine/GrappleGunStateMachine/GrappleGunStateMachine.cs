using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GrappleGunStateMachine : MonoBehaviourStateMachine
{
    GrappleGun grappleGun;

    public GrappleGunStateMachine(GrappleGun grappleGun)
    {
        this.grappleGun = grappleGun;
    }

    public void TransitionToState(GrappleGunState nextState)
    {
        base.TransitionToState(nextState);
    }

    // Create events here that states can add/remove themselves as listeners to
}
