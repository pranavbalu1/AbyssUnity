using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private GameObject slotHolder;
    
    public List<ItemClass> inventory = new List<ItemClass>();
    
    private GameObject[] slots;

    public void Start()
    {
       slots = new GameObject[slotHolder.transform.childCount];
        for (int i = 0; i < slotHolder.transform.childCount; i++)
        {
            slots[i] = slotHolder.transform.GetChild(i).gameObject;
        }

        RefreshUI();
    }

    public void RefreshUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            try 
            { 
            slots[i].transform.GetChild(0).GetComponent<Image>().sprite = inventory[i].item_icon;
            }
            catch
            {
                slots[i].transform.GetChild(0).GetComponent<Image>().sprite = null;
                slots[i].transform.GetChild(0).GetComponent<Image>().enabled = false;
            }
        }
    }

    public void AddItem(ItemClass item)
    {
        inventory.Add(item);
        RefreshUI();
    }
    public void RemoveItem(ItemClass item) 
    {
        inventory.Remove(item);
        RefreshUI();

    }
}
