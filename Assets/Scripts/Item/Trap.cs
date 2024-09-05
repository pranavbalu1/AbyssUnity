using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class TrapTrigger : MonoBehaviour
{
    private bool actOnPlayer = false;
    private bool actOnEnemy = true;

    void OnTriggerEnter(Collider collider)
    {
        GameObject otherObj = collider.gameObject;
        //Debug.Log("Triggered enter with: " + otherObj);

        if (otherObj.CompareTag("Player") || otherObj.CompareTag("Enemy"))
        {
            //Debug.Log("Triggered enter with: " + otherObj);

            // Check if the object has the PlayerController component
            PlayerController playerController = otherObj.GetComponent<PlayerController>();
            if (otherObj.CompareTag("Player") && actOnPlayer && playerController != null)
            {
                playerController.SetStickyMovement();
                playerController.IsTrapped(true);
            }
            else if (otherObj.CompareTag("Player") && playerController == null)
            {
                Debug.LogError("PlayerController component is missing on the Player object.");
            }

            // Check if the object has the EnemyAI component
            EnemyAI enemyAI = otherObj.GetComponent<EnemyAI>();
            //TODO: need to add functionality with different classes of enemies
            if (otherObj.CompareTag("Enemy") && actOnEnemy && enemyAI != null)
            {
                //Debug.Log("Enemy is trapped");
                enemyAI.SetIsTrapped(true);
            }
            else if (otherObj.CompareTag("Enemy") && enemyAI == null)
            {
                Debug.LogError("EnemyAI component is missing on the Enemy object.");
            }
        }
    }


    private void OnTriggerStay(Collider collider)
    {
        GameObject otherObj = collider.gameObject;
        //Trigger some animation
        if (otherObj.CompareTag("Player") || otherObj.CompareTag("Enemy"))
        {
            //Debug.Log("Triggered stay with: " + otherObj);
            if (otherObj.CompareTag("Player") && actOnPlayer)
            {
                // Handle player stay logic
            }

            if (otherObj.CompareTag("Enemy") && actOnEnemy)
            {
                // Handle enemy stay logic
            }
        }
    }

    void OnTriggerExit(Collider collider)
    {
        GameObject otherObj = collider.gameObject;

        if (otherObj.CompareTag("Player") || otherObj.CompareTag("Enemy"))
        {
            //Debug.Log("Triggered exit with: " + otherObj);
            if (otherObj.CompareTag("Player") && actOnPlayer)
            {
                otherObj.GetComponent<PlayerController>().SetNormalMovement();
                otherObj.GetComponent<PlayerController>().IsTrapped(false);
            }

            if (otherObj.CompareTag("Enemy") && actOnEnemy)
            {
                // Handle enemy exit logic
                otherObj.GetComponent<EnemyAI>().SetIsTrapped(false);

            }
        }
    }
}
