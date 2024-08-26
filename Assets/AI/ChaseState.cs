using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : State
{
    public IdleState idleState;
    public bool canSeePlayer;

    public override State RunCurrentState()
    {
        
        Debug.Log("Chase State");
        if (!canSeePlayer)
        {
           return idleState; 
        }
        else
        {
            return this;
        }
    }
}
