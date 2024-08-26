using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : State
{
    public bool canSeePlayer;
    public ChaseState ChaseState;
    public override State RunCurrentState()
    {
        Debug.Log("Idle State");
        if (canSeePlayer)
        {
            return ChaseState;
        }
        else if (canSeePlayer == false)
        {
            return this;
        }
        return this;
    }
}
