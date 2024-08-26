using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideState : State
{
    public override State RunCurrentState()
    {
        Debug.Log("Hide State");
        return this;
    }
}
