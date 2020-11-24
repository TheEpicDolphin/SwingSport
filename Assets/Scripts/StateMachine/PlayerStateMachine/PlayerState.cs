using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerState : GameObjectState
{
    protected Player player;
    protected PlayerStateMachine playerSM;

    public PlayerState(PlayerStateMachine playerSM, Player player) : base()
    {
        this.player = player;
        this.playerSM = playerSM;
    }
}
