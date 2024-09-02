using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private GameObject slotHolder;
    [SerializeField] private GameObject slotSelector;
    [SerializeField] private int selectedSlotIndex = 0;
    public ItemClass selectedItem;
    
public List<ItemClass> inventory = new(4);
    

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

    public void Update()
    {
        //scroll wheel to select slot
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            selectedSlotIndex--;
            if (selectedSlotIndex < 0)
            {
                selectedSlotIndex = slots.Length - 1;
            }
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            selectedSlotIndex++;
            if (selectedSlotIndex >= slots.Length)
            {
                selectedSlotIndex = 0;
            }
        }

        //1 through 4 to select slot
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedSlotIndex = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectedSlotIndex = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            selectedSlotIndex = 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            selectedSlotIndex = 3;
        }

        slotSelector.transform.position = slots[selectedSlotIndex].transform.position;
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

    public ItemClass GetSelectedItem => inventory[selectedSlotIndex];
}
