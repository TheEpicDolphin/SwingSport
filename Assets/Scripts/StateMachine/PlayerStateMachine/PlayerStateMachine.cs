using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerStateMachine : GameObjectStateMachine
{
    public PlayerStateMachine(Player player, List<Type> stateTypes)
    {
        foreach (Type stateType in stateTypes)
        {
            stateMap[stateType] = (PlayerState)Activator.CreateInstance(stateType, new object[] { this, player });
        }
    }

    public new void InitWithState<T>() where T : PlayerState
    {
        base.InitWithState<T>();
    }

    public new void TransitionToState<T>() where T : PlayerState
    {
        base.TransitionToState<T>();
    }
}
