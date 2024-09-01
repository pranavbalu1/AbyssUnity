using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public List<ItemClass> inventory = new List<ItemClass>();

    public void Start()
    {
       
    }

    public void AddItem(ItemClass item)
    {
        inventory.Add(item);
    }
    public void RemoveItem(ItemClass item) 
    {
        inventory.Remove(item);
    }
}
