using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Trap Class", menuName = "Item/Trap")]
public class TrapClass : ItemClass
{
    //Data Specific to Trap

    [Header("Trap")]
    public TrapType trapType;
    public enum TrapType
    {
       red,
       green,
       orange
    }

    public override ItemClass GetItem() => this;
    public override TrapClass GetTrap() => this;
    public override ConsumableClass GetConsumable() => null;

}
