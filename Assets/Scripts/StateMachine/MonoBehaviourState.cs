using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMonoBehaviourState
{
    void OnEnter();

    void UpdateStep();

    void FixedUpdateStep();

    void OnExit();
}
