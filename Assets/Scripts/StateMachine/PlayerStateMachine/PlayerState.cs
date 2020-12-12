using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerState : IMonoBehaviourState
{
    protected readonly Player player;

    public PlayerState(Player player) : base()
    {
        this.player = player;
    }

    public abstract void OnEnter();

    public abstract void FixedUpdateStep();

    public abstract void UpdateStep();

    public abstract void OnExit();
}
