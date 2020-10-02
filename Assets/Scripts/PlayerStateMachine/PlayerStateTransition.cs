using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateTransition
{
    PlayerState nextState;
    public PlayerStateTransition(PlayerState nextState)
    {
        this.nextState = nextState;
    }

    public bool EvaluateCondition()
    {

    }
}
