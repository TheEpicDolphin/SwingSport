using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MonoBehaviourStateMachine
{
    protected IMonoBehaviourState currentState;
    protected HashSet<Type> allowedStates = new HashSet<Type>();

    public void InitWithState(IMonoBehaviourState state)
    {
        currentState = state;
        currentState.OnEnter();
    }

    public void TransitionToState(IMonoBehaviourState nextState)
    {
        currentState.OnExit();
        currentState = nextState;
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
