using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerStateMachine
{
    PlayerState currentState;
    private Dictionary<Type, PlayerState> stateMap = new Dictionary<Type, PlayerState>();

    public PlayerStateMachine(Player player, List<Type> stateTypes)
    {
        foreach (Type stateType in stateTypes)
        {
            stateMap[stateType] = (PlayerState) Activator.CreateInstance(stateType, new object[]{ this, player });
        }
    }

    public void InitWithState<T>() where T : PlayerState
    {
        currentState = GetState<T>();
        currentState.OnEnter();
    }

    private PlayerState GetState<T>() where T : PlayerState
    {
        return stateMap[typeof(T)];
    }

    public void TransitionToState<T>() where T : PlayerState
    {
        currentState.OnExit();
        currentState = GetState<T>();
        currentState.OnEnter();
    }

    public void UpdateStep()
    {
        currentState.UpdateStep();
    }

    public void FixedUpdateStep()
    {
        currentState.FixedUpdateStep();
    }
}
