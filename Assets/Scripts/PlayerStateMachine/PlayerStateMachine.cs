using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerStateMachine
{
    PlayerState currentState;
    private Dictionary<Type, PlayerState> stateMap = new Dictionary<Type, PlayerState>();

    public PlayerStateMachine(List<Type> stateTypes)
    {
        foreach (Type stateType in stateTypes)
        {
            stateMap[stateType] = (stateType) Activator.CreateInstance(stateType);
        }
    }

    public T GetState<T>() where T : PlayerState
    {
        return stateMap[typeof(T)];
    }

    public void TransitionToState(Type stateType)
    {
        currentState.OnExit();
        currentState = GetState<stateType>();
        currentState.OnEnter();
    }

    public void UpdateStep(Player player)
    {
        currentState = currentState.UpdateStep(player);
    }

    public void FixedUpdateStep(Player player)
    {
        currentState = currentState.FixedUpdateStep(player);
    }
}
