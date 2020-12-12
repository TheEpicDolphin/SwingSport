using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GrappleGunState : IMonoBehaviourState
{
    protected readonly GrappleGun grappleGun;

    public GrappleGunState(GrappleGun grappleGun) : base()
    {
        this.grappleGun = grappleGun;
    }

    public abstract void OnEnter();

    public abstract void FixedUpdateStep();

    public abstract void UpdateStep();

    public abstract void OnExit();
    
}
