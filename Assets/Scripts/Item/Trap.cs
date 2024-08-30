using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class TrapTrigger : MonoBehaviour
{

    private bool actOnPLayer = false;
    private bool actOnEnemy = true;

    void OnTriggerEnter(Collider collider)
    {
        GameObject otherObj = collider.gameObject;

        if (otherObj.CompareTag("Player") || otherObj.CompareTag("Enemy"))
        {
            Debug.Log("Triggered enter with: " + otherObj);
            if (otherObj.CompareTag("Player") && actOnPLayer)
            {
                otherObj.GetComponent<PlayerController>().setStickyMovement();
                otherObj.GetComponent<PlayerController>().isTrapped(true);
            }
            
            if (otherObj.CompareTag("Enemy") && actOnEnemy)
            {
                Debug.Log("Enemy is trapped");
                otherObj.GetComponent<EnemyAI>().isTrapped();
            }
        }
    }

    private void OnTriggerStay(Collider collider)
    {
        GameObject otherObj = collider.gameObject;
        //Trigger some animation
        if (collider.gameObject.CompareTag("Player") || otherObj.CompareTag("Enemy"))
        {
            Debug.Log("Triggered stay with: " + collider.gameObject);
            if (otherObj.CompareTag("Player") && actOnPLayer )
            {
                // Handle player stay logic
               // otherObj.GetComponent<PlayerController>().ReduceSpeed(0.1f);
            }

            if (otherObj.CompareTag("Enemy") && actOnEnemy)
            {
                //otherObj.GetComponent<EnemyAI>().enemySpeed = 0.1f;
            }
        }
    }

    void OnTriggerExit(Collider collider)
    {
        GameObject otherObj = collider.gameObject;

        if (otherObj.CompareTag("Player") || otherObj.CompareTag("Enemy"))
        {
            Debug.Log("Triggered exit with: " + otherObj);
            if (otherObj.CompareTag("Player") && actOnPLayer)
            {
                otherObj.GetComponent<PlayerController>().setNormalMovement();
                otherObj.GetComponent<PlayerController>().isTrapped(false);
            }
            
            if (otherObj.CompareTag("Enemy") && actOnEnemy)
            {
                //otherObj.GetComponent<EnemyAI>().isTrapped = false;
            }
        }
    }
}
