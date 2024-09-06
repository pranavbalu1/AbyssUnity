using UnityEngine;
using UnityEngine.AI;

public class MimicEnemy : EnemyBase
{
    public float activateRange = 5f;
    public float deactivateRange = 20f;
    public float revealDistance = 2f;
    public GameObject[] disguiseObjects;
    public float fieldOfViewAngle = 90f;

    private bool playerInActivateRange = false;
    private bool isRevealed = false;
    private GameObject currentDisguise;

    private SleepState sleepState = new SleepState();
    private RevealState revealState = new RevealState();
    private ChaseState chaseState = new ChaseState();

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

        if (player != null)
        {
            playerInActivateRange = Physics.CheckSphere(transform.position, activateRange, whatIsPlayer);
        }
    }

    private class SleepState : EnemyBase.EnemyState
    {
        public override void EnterState(EnemyBase enemy)
        {
            MimicEnemy mimic = (MimicEnemy)enemy;
            Debug.Log("Entering Sleep State");
            mimic.ChooseRandomDisguise();
            mimic.gameObject.SetActive(false); // Hide the actual enemy object
        }

        public override void UpdateState(EnemyBase enemy)
        {
            MimicEnemy mimic = (MimicEnemy)enemy;

            if (mimic.agent.remainingDistance < 0.5f)
            {
                mimic.agent.isStopped = true;
            }

            mimic.playerInActivateRange = Physics.CheckSphere(mimic.transform.position, mimic.activateRange, mimic.whatIsPlayer);

            if (mimic.playerInActivateRange)
            {
                mimic.TransitionToState(mimic.revealState);
            }
        }
    }

    private class RevealState : EnemyBase.EnemyState
    {
        private float teleportCooldown = 3f;
        private float teleportTimer = 0f;
        private int maxTeleportAttempts = 3;
        private int currentTeleportAttempts = 0;
        private Vector3 destination;

        public override void EnterState(EnemyBase enemy)
        {
            MimicEnemy mimic = (MimicEnemy)enemy;
            Debug.Log("Revealing Mimic");
            mimic.isRevealed = true;
            mimic.gameObject.SetActive(true);

            if (mimic.currentDisguise != null)
            {
                GameObject.Destroy(mimic.currentDisguise);
            }

            mimic.agent.speed = mimic.enemySpeed;
            mimic.agent.acceleration = mimic.enemyAcceleration;

            // Start the teleportation process
            teleportTimer = teleportCooldown;
            AttemptTeleport(mimic);
        }

        public override void UpdateState(EnemyBase enemy)
        {
            MimicEnemy mimic = (MimicEnemy)enemy;

            // Handle teleport cooldown and update teleport timer
            if (teleportTimer > 0)
            {
                teleportTimer -= Time.deltaTime;
                return;
            }

            if (mimic.agent.remainingDistance < 0.5f)
            {
                // Check if the player is obstructed at the new position
                if (mimic.IsPlayerObstructed())
                {
                    mimic.TransitionToState(mimic.sleepState);
                }
                else
                {
                    // If not obstructed, keep moving to the player or handle other logic
                    float distanceToPlayer = Vector3.Distance(mimic.transform.position, mimic.player.position);
                    if (distanceToPlayer > mimic.deactivateRange)
                    {
                        mimic.TransitionToState(mimic.sleepState);
                    }
                }
            }

            // Retry teleportation if needed
            if (currentTeleportAttempts < maxTeleportAttempts && !mimic.IsPlayerObstructed())
            {
                AttemptTeleport(mimic);
            }
            else if (currentTeleportAttempts >= maxTeleportAttempts)
            {
                mimic.TransitionToState(mimic.chaseState);
            }
        }

        private void AttemptTeleport(MimicEnemy mimic)
        {
            currentTeleportAttempts++;
            Vector3 randomPosition = GetRandomPositionAwayFromPlayer(mimic);

            // Check if the position is valid for teleportation
            if (randomPosition != Vector3.zero)
            {
                mimic.agent.Warp(randomPosition);
                destination = randomPosition;
                mimic.agent.SetDestination(destination);
            }
            else
            {
                Debug.LogWarning("Teleportation failed. No valid position found.");
            }
        }

        private Vector3 GetRandomPositionAwayFromPlayer(MimicEnemy mimic)
        {
            // Generate a random point away from the player within a reasonable distance
            Vector3 randomDirection = Random.insideUnitSphere * 10f; // Adjust distance as needed
            randomDirection += mimic.player.position;
            NavMeshHit navHit;

            if (NavMesh.SamplePosition(randomDirection, out navHit, 1f, NavMesh.AllAreas))
            {
                return navHit.position;
            }

            return Vector3.zero;
        }
    }


    private class ChaseState : EnemyBase.EnemyState
    {
        public override void EnterState(EnemyBase enemy)
        {
            MimicEnemy mimic = (MimicEnemy)enemy;
            Debug.Log("Entering Chase State");
            mimic.agent.isStopped = false;
        }

        public override void UpdateState(EnemyBase enemy)
        {
            MimicEnemy mimic = (MimicEnemy)enemy;

            if (mimic.player == null) return;

            float distanceToPlayer = Vector3.Distance(mimic.transform.position, mimic.player.position);

            if (distanceToPlayer > mimic.enemyReach)
            {
                mimic.agent.SetDestination(mimic.player.position);
                mimic.agent.isStopped = false;
            }
            else
            {
                mimic.agent.isStopped = true;
                mimic.agent.SetDestination(mimic.transform.position);
            }

            if (distanceToPlayer > mimic.deactivateRange)
            {
                mimic.TransitionToState(mimic.sleepState);
            }
        }
    }

    private void ChooseRandomDisguise()
    {
        if (disguiseObjects.Length == 0) return;

        if (currentDisguise != null)
        {
            GameObject.Destroy(currentDisguise);
        }

        currentDisguise = Instantiate(disguiseObjects[Random.Range(0, disguiseObjects.Length)], transform.position, Quaternion.identity);
        currentDisguise.transform.SetParent(transform);
    }
}
