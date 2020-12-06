using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GrappleGunStateMachine : GameObjectStateMachine
{
    public GrappleGunStateMachine(GrappleGun grappleGun, List<Type> stateTypes)
    {
        foreach (Type stateType in stateTypes)
        {
            stateMap[stateType] = (GrappleGunState)Activator.CreateInstance(stateType, new object[] { this, grappleGun });
        }
    }

    public new void InitWithState<T>() where T : GrappleGunState
    {
        base.InitWithState<T>();
    }

    public new void TransitionToState<T>() where T : GrappleGunState
    {
        base.TransitionToState<T>();
    }

    // Create events here that states can add/remove themselves as listeners to
}
