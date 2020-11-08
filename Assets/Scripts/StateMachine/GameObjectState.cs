using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameObjectState
{
    protected GameObject gameObject;

    protected GameObjectStateMachine gameObjectSM;

    public GameObjectState(GameObjectStateMachine gameObjectSM, GameObject gameObject)
    {
        this.gameObjectSM = gameObjectSM;
        this.gameObject = gameObject;
    }

    public abstract void OnEnter();

    public abstract void UpdateStep();

    public abstract void FixedUpdateStep();

    public abstract void OnExit();
}
