using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Consumable Class", menuName = "Item/Consumable")]
public class ConsumableClass : ItemClass
{
    //Data Specific to Consumable

    [Header("Consumable")]
    public ConsumableType consumableType;
    public enum ConsumableType
    {
        health,
        stamina

    }
    public override ItemClass GetItem() => this;
    public override TrapClass GetTrap() => null;
    public override ConsumableClass GetConsumable() => this;

}
