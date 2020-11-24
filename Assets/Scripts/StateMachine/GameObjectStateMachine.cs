using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class GameObjectStateMachine
{
    protected GameObjectState currentState;
    protected Dictionary<Type, GameObjectState> stateMap = new Dictionary<Type, GameObjectState>();

    protected void InitWithState<T>() where T : GameObjectState
    {
        currentState = GetState<T>();
        currentState.OnEnter();
    }

    private GameObjectState GetState<T>() where T : GameObjectState
    {
        return stateMap[typeof(T)];
    }

    protected void TransitionToState<T>() where T : GameObjectState
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
