using UnityEngine;
using UnityEngine.AI;

public class MimicEnemy : EnemyBase
{
    public GameObject disguiseObject; // Prefab for the disguise object
    public float spawnHeight = 2f; // How far above the enemy the disguise spawns
    public float detectionRange = 5f; // Range within which the player will trigger the reveal
    public float chaseChance = 0.5f; // Chance to move to ChaseState (50% in this case)
    public float warpCooldown = 2f; // Cooldown between each warp

    private GameObject currentDisguise;
    private SleepState sleepState = new SleepState();
    private RevealState revealState = new RevealState();
    private ChaseState chaseState = new ChaseState(); // New ChaseState
    private float lastWarpTime; // Time of the last warp

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
        }

        public override void UpdateState(EnemyBase enemy)
        {
            MimicEnemy mimic = (MimicEnemy)enemy;
            // Teleport to a hidden location if the player is obstructed and cooldown has passed
            if (!mimic.IsPlayerObstructed() && Time.time - mimic.lastWarpTime >= mimic.warpCooldown)
            {
                Vector3 teleportPosition = FindHiddenLocation(mimic);
                if (teleportPosition != mimic.transform.position)
                {
                    mimic.agent.Warp(teleportPosition); // Teleport the enemy using NavMeshAgent
                    mimic.lastWarpTime = Time.time; // Update the last warp time
                    Debug.Log("Teleporting to a hidden location!");
                }
                else
                {
                    mimic.TransitionToState(mimic.chaseState);
                }
            }
            Debug.Log("Reveal Update");
        }

        private Vector3 FindHiddenLocation(MimicEnemy mimic)
        {
            // Find a random hidden location where the enemy will teleport to
            Vector3 randomDirection = Random.insideUnitSphere * 10f; // Adjust teleport radius
            randomDirection += mimic.transform.position;

            // Ensure the teleport position is valid by checking the NavMesh
            if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, 10f, NavMesh.AllAreas))
            {
                Debug.Log("Valid teleport position found! Position: " + hit.position);
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

    private class ChaseState : EnemyBase.EnemyState
    {
        public override void EnterState(EnemyBase enemy)
        {
            MimicEnemy mimic = (MimicEnemy)enemy;
            Debug.Log("Entering Chase State");
        }

        public override void UpdateState(EnemyBase enemy)
        {
            MimicEnemy mimic = (MimicEnemy)enemy;
            float distanceToPlayer = Vector3.Distance(mimic.transform.position, mimic.player.position);

            Debug.Log("Chasing player");

            if (distanceToPlayer > mimic.enemyReach)
            {
                mimic.agent.isStopped = false;
                mimic.agent.SetDestination(mimic.player.position);
            }
            else
            {
                mimic.agent.isStopped = true;
                mimic.agent.SetDestination(mimic.transform.position);
                mimic.TransitionToState(mimic.revealState);
            }
        }
    }
}
