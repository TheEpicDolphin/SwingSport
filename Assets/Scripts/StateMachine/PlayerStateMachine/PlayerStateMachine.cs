using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerStateMachine : MonoBehaviourStateMachine
{
    Player player;

    public PlayerStateMachine(Player player)
    {
        this.player = player;
    }

    public void TransitionToState(PlayerState nextState)
    {
        base.TransitionToState(nextState);
    }

}
