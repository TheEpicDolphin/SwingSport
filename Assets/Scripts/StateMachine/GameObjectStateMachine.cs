using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class GameObjectStateMachine
{
    GameObjectState currentState;
    private Dictionary<Type, GameObjectState> stateMap = new Dictionary<Type, GameObjectState>();

    public GameObjectStateMachine(object context, List<Type> stateTypes)
    {
        foreach (Type stateType in stateTypes)
        {
            stateMap[stateType] = (GameObjectState)Activator.CreateInstance(stateType, new object[] { this, context });
        }
    }

    public void InitWithState<T>() where T : GameObjectState
    {
        currentState = GetState<T>();
        currentState.OnEnter();
    }

    private GameObjectState GetState<T>() where T : GameObjectState
    {
        return stateMap[typeof(T)];
    }

    public void TransitionToState<T>() where T : GameObjectState
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
