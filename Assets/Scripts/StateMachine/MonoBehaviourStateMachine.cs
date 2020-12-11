using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class MonoBehaviourStateMachine
{
    protected MonoBehaviourState currentState;
    protected HashSet<Type> allowedStates = new HashSet<Type>();

    public void InitWithState(MonoBehaviourState state)
    {
        currentState = state;
        currentState.OnEnter();
    }

    public void TransitionToState(MonoBehaviourState nextState)
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
