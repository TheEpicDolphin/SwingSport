using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerState
{
    protected Player player;

    protected PlayerStateMachine playerSM;

    public PlayerState(Player player)
    {
        this.player = player;
    }

    public abstract void OnEnter();

    public abstract void UpdateStep();

    public abstract void FixedUpdateStep();

    public abstract void OnExit();
}
