using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GrappleGunState : MonoBehaviourState
{
    protected GrappleGun grappleGun;
    protected GrappleGunStateMachine grappleGunSM;

    public GrappleGunState(GrappleGunStateMachine grappleGunSM, GrappleGun grappleGun) : base()
    {
        this.grappleGun = grappleGun;
        this.grappleGunSM = grappleGunSM;
    }
}
