using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState
{
    List<PlayerStateTransition> transitions;
    public PlayerState()
    {

    }

    public void Run()
    {
        /* Check to see if any transitions should occur */
        foreach(PlayerStateTransition transition in transitions)
        {
            
        }

        /* Perform state action */
    }

    
}
