using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Trap Class", menuName = "Item/Trap")]
public class TrapClass : ItemClass
{
    [Header("Trap")]
    public TrapType trapType;
    public GameObject trapPrefab; // Reference to the trap prefab

    public enum TrapType
    {
        Red,
        Green,
        Orange
    }

    public override void UseItem(PlayerController caller)
    {
        base.UseItem(caller);
        PlaceTrap(caller);
    }

    private void PlaceTrap(PlayerController caller)
    {
        // Instantiate the trap prefab at the player's position with the player's rotation
        GameObject trap = Instantiate(trapPrefab, caller.transform.position + caller.transform.forward, caller.transform.rotation);
        TrapTrigger trapTrigger = trap.GetComponent<TrapTrigger>();

        if (trapTrigger == null)
        {
            Debug.LogError("Trap prefab is missing the TrapTrigger component.");
        }
        else
        {
            // Customize the trap trigger behavior based on trap type if needed
            // For example, different trap types could have different effects or visuals
        }
    }
}
