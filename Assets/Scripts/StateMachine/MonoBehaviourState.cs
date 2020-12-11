using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonoBehaviourState
{
    public abstract void OnEnter();

    public abstract void UpdateStep();

    public abstract void FixedUpdateStep();

    public abstract void OnExit();
}
