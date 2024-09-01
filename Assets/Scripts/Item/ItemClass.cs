using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemClass : ScriptableObject
{
    //Data shared to every Item
    [Header("Item")]
    public string item_name;
    public Sprite item_icon;

    public abstract ItemClass GetItem();
    public abstract TrapClass GetTrap();
    public abstract ConsumableClass GetConsumable();


}

