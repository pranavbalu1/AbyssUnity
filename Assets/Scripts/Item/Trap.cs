using UnityEngine;

public class TrapTrigger : MonoBehaviour
{
    private bool actOnPlayer = false;
    private bool actOnEnemy = true;

    void OnTriggerEnter(Collider collider)
    {
        GameObject otherObj = collider.gameObject;

        if (otherObj.CompareTag("Player") || otherObj.CompareTag("Enemy"))
        {
            // Handle Player trap logic
            PlayerController playerController = otherObj.GetComponent<PlayerController>();
            if (otherObj.CompareTag("Player") && actOnPlayer && playerController != null)
            {
                playerController.SetStickyMovement();
                playerController.IsTrapped(true);
            }

            // Handle Enemy trap logic (works with both EnemyAI and Enemy2)
            EnemyBase enemy = otherObj.GetComponent<EnemyBase>();
            if (otherObj.CompareTag("Enemy") && actOnEnemy && enemy != null)
            {
                enemy.SetIsTrapped(true);
            }
        }
    }

    void OnTriggerExit(Collider collider)
    {
        GameObject otherObj = collider.gameObject;

        if (otherObj.CompareTag("Player") || otherObj.CompareTag("Enemy"))
        {
            if (otherObj.CompareTag("Player") && actOnPlayer)
            {
                otherObj.GetComponent<PlayerController>().SetNormalMovement();
                otherObj.GetComponent<PlayerController>().IsTrapped(false);
            }

            // Handle Enemy exit logic
            EnemyBase enemy = otherObj.GetComponent<EnemyBase>();
            if (otherObj.CompareTag("Enemy") && actOnEnemy && enemy != null)
            {
                enemy.SetIsTrapped(false);
            }
        }
    }
}
