using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerState
{
    public abstract void OnEnter();

    public abstract PlayerState UpdateStep(Player player);

    public abstract PlayerState FixedUpdateStep(Player player);
}
