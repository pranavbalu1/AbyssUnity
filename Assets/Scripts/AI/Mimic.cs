using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MimicEnemy : EnemyBase
{
    public GameObject disguiseObject; // Prefab for the disguise object
    public float spawnHeight = 2f; // How far above the enemy the disguise spawns
    public float detectionRange = 5f; // Range within which the player will trigger the reveal

    private GameObject currentDisguise;
    private SleepState sleepState = new SleepState();
    private RevealState revealState = new RevealState();

    private void Start()
    {
        if (player != null)
        {
            TransitionToState(sleepState);
        }
    }

    protected override void Update()
    {
        base.Update();
    }

    // Method to instantiate the disguise object with a Rigidbody and spawn above the enemy
    public void TransformIntoDisguise(GameObject disguisePrefab)
    {
        if (currentDisguise != null)
        {
            GameObject.Destroy(currentDisguise); // Destroy the current disguise if it already exists
        }

        Debug.Log("Transforming into disguise");

        Vector3 spawnPosition = transform.position + Vector3.up * spawnHeight;
        currentDisguise = Instantiate(disguisePrefab, spawnPosition, Quaternion.identity);
        currentDisguise.SetActive(true);
        currentDisguise.transform.localScale = transform.localScale;

        Rigidbody rb = currentDisguise.AddComponent<Rigidbody>();
        rb.useGravity = true;
        rb.mass = 1f; // Adjust the mass as necessary for physics

        currentDisguise.transform.SetParent(null);
    }

    private class SleepState : EnemyBase.EnemyState
    {
        public override void EnterState(EnemyBase enemy)
        {
            MimicEnemy mimic = (MimicEnemy)enemy;
            Debug.Log("Entering Sleep State");

            mimic.TransformIntoDisguise(mimic.disguiseObject);
            mimic.SetVisibility(false);
            mimic.SetCollision(false);
        }

        public override void UpdateState(EnemyBase enemy)
        {
            MimicEnemy mimic = (MimicEnemy)enemy;

            // Check if the player is within detection range
            float distanceToPlayer = Vector3.Distance(mimic.transform.position, mimic.player.position);
            if (distanceToPlayer <= mimic.detectionRange)
            {
                // Transition to RevealState if the player is nearby
                mimic.TransitionToState(mimic.revealState);
            }
        }
    }

    private class RevealState : EnemyBase.EnemyState
    {
        private float teleportCooldown = 3f;
        private float teleportChance = 0.5f; // 50% chance to teleport
        private bool hasTeleported = false;
        private float timeSinceReveal = 0f; // Track the time spent in RevealState

        public override void EnterState(EnemyBase enemy)
        {
            MimicEnemy mimic = (MimicEnemy)enemy;
            Debug.Log("Entering Reveal State");

            // Make the enemy visible and collidable again
            mimic.SetVisibility(true);
            mimic.SetCollision(true);
            if (mimic.currentDisguise != null)
            {
                mimic.currentDisguise.SetActive(false);
            }

            // Reset state variables
            hasTeleported = false;
            timeSinceReveal = 0f;
        }

        public override void UpdateState(EnemyBase enemy)
        {
            MimicEnemy mimic = (MimicEnemy)enemy;

            // Update the timer
            timeSinceReveal += Time.deltaTime;

            // Check if teleport cooldown has passed
            if (timeSinceReveal >= teleportCooldown && !hasTeleported)
            {
                if (Random.value <= teleportChance)
                {
                    if (mimic.IsPlayerObstructed()) 
                    {
                        // Perform teleportation logic here
                        Debug.Log("Player is obstructed. Teleporting to a new hidden location!");

                        Vector3 teleportPosition = FindHiddenLocation(mimic);
                        mimic.agent.Warp(teleportPosition); // Teleport the enemy using NavMeshAgent
                        hasTeleported = true;
                    }
                    else
                    {
                        Debug.Log("Player is not obstructed. No teleportation.");
                    }
                }
            }
        }

        private Vector3 FindHiddenLocation(MimicEnemy mimic)
        {
            // Find a random hidden location where the enemy will teleport to
            Vector3 randomDirection = Random.insideUnitSphere * 10f; // Adjust teleport radius
            randomDirection += mimic.transform.position;

            // Ensure the teleport position is valid by checking the NavMesh
            if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, 10f, NavMesh.AllAreas))
            {
                Debug.Log("Valid teleport position found!");
                return hit.position;
            }
            else
            {
                Debug.Log("No valid teleport position found. Using current position.");
                // If no valid position is found, return the current position as a fallback
                return mimic.transform.position;
            }
        }
    }
}
