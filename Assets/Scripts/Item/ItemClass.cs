using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemClass : ScriptableObject
{
    //Data shared to every Item
    [Header("Item")]
    public string item_name;
    public Sprite item_icon;

    public virtual ItemClass GetItem() { return this; }
    public virtual TrapClass GetTrap() { return null; }
    public virtual ConsumableClass GetConsumable() { return null; }

    public virtual void UseItem(PlayerController caller)
    {
        Debug.Log("Used " + item_name);
    }


}

