using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer, whatIsObstacle;
    public float health = 10;  // Set a default health value

    //Enemy Attributes
    public float sightRange = 20f;
    public bool playerInSightRange = false;
    public float enemySpeed = 5f;
    public float enemyAcceleration = 8f;
    public float enemyReach = 2f;
    public bool isTrapped = false;



    private EnemyState currentState;

    private PatrolState patrolState = new PatrolState();
    private ChaseState chaseState = new ChaseState();
    private HideState hideState = new HideState();
    private TrappedState trappedState = new TrappedState();

    private void Awake()
    {
        //player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        agent.speed = enemySpeed;
        agent.acceleration = enemyAcceleration;
    }

    private void Start()
    {
        currentState = patrolState;
        currentState.EnterState(this);
    }

    private void Update()
    {
        // Update current state logic
        currentState.UpdateState(this);

        // Check for sight range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);

        // Transition based on player visibility
        if (!playerInSightRange && currentState != patrolState)
            TransitionToState(patrolState);
        else if (playerInSightRange && currentState != hideState)
            TransitionToState(hideState);

        if (isTrapped)
        {
            TransitionToState(trappedState);
        }
    }


 


    public void TransitionToState(EnemyState state)
    {
        currentState = state;
        currentState.EnterState(this);
    }

    // Base state class
    public abstract class EnemyState
    {
        public abstract void EnterState(EnemyAI enemy);
        public abstract void UpdateState(EnemyAI enemy);
    }

    // Patrol state class
    private class PatrolState : EnemyState
    {
        public override void EnterState(EnemyAI enemy)
        {
            Debug.Log("Entering Patrol State");
        }

        public override void UpdateState(EnemyAI enemy)
        {
            if (enemy.agent.remainingDistance < 5f)
            {
                float x = Random.Range(-11, 11);
                float z = Random.Range(-11, 11);
                Vector3 position = new Vector3(x, 0f, z);
                enemy.agent.SetDestination(position);
                //Debug.Log($"Patrolling to {position}");
            }
        }
    }

    //Hide State class
    private class HideState : EnemyState
    {
        public override void EnterState(EnemyAI enemy)
        {
            Debug.Log("Entering Hide State");
            Vector3 hideSpot = FindHidingSpot(enemy);
            if (hideSpot == enemy.transform.position)
            {
                Debug.Log("No hiding spots found. Moving to Chase State.");
                enemy.TransitionToState(enemy.chaseState);
            }
            enemy.agent.SetDestination(hideSpot);
            enemy.agent.isStopped = false;

            Debug.Log($"Hiding at {hideSpot}");
        }

        public override void UpdateState(EnemyAI enemy)
        {
            Debug.Log("Hiding Update");
            if (enemy.agent.remainingDistance < 0.5f)
            {
                Debug.Log("Enemy is hiding.");
                enemy.agent.isStopped = true;
            }
        }
        private Vector3 FindHidingSpot(EnemyAI enemy)
        {
            // Find obstacles within the search radius
            Collider[] obstacles = Physics.OverlapSphere(enemy.transform.position, enemy.sightRange, enemy.whatIsObstacle);
            Vector3 bestHidingSpot = enemy.transform.position;
            float bestHidingSpotDistance = Mathf.Infinity;

            foreach (var obstacle in obstacles)
            {
                // Calculate a position behind the obstacle relative to the player's position
                Vector3 directionToPlayer = (enemy.player.position - obstacle.transform.position).normalized;
                Vector3 potentialHidingSpot = obstacle.transform.position - directionToPlayer * 2f;

                // Check if the potential hiding spot is occluded from the player's line of sight
                if (IsOccludedFromPlayer(potentialHidingSpot, enemy))
                {
                    float distanceToPlayer = Vector3.Distance(potentialHidingSpot, enemy.player.position);
                    if (distanceToPlayer < bestHidingSpotDistance)
                    {
                        bestHidingSpot = potentialHidingSpot;
                        bestHidingSpotDistance = distanceToPlayer;
                    }
                }
            }
            
            // If no good hiding spots are found, default to a random position
            if (bestHidingSpot == enemy.transform.position)
            {
                
                Debug.Log("No hiding spots found.");
            }

            return bestHidingSpot;
        }
        private bool IsOccludedFromPlayer(Vector3 spot, EnemyAI enemy)
        {
            Vector3 directionToPlayer = enemy.player.position - spot;
            Ray ray = new Ray(spot, directionToPlayer);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, directionToPlayer.magnitude))
            {
                if (hit.transform != enemy.player)
                {
                    // If the ray hits something that is not the player, the spot is occluded
                    return true;
                }
            }

            // If the ray hits the player directly, the spot is not occluded
            return false;
        }
    }

    // Chase state class
    private class ChaseState : EnemyState
    {
        public override void EnterState(EnemyAI enemy)
        {
            Debug.Log("Entering Chase State");
            enemy.agent.isStopped = false;  // Ensure the agent starts moving again if stopped
        }

        public override void UpdateState(EnemyAI enemy)
        {
            float distanceToPlayer = Vector3.Distance(enemy.transform.position, enemy.player.position);

            if (distanceToPlayer > enemy.enemyReach)
            {
                enemy.agent.isStopped = false;
                enemy.agent.SetDestination(enemy.player.position);
                Debug.Log($"Chasing Player {enemy.player.position}");
            }
            else
            {
                enemy.agent.isStopped = true;
                enemy.agent.SetDestination(enemy.transform.position);  // Stop the agent at the current position
                Debug.Log($"Chasing Player {enemy.player.position}. Slowing down.");
            }
        }
    }

    private class TrappedState : EnemyState
    {
        public override void EnterState(EnemyAI enemy)
        {
            Debug.Log("Entering Trapped State");
            enemy.agent.speed *= 0.1f;
        }

        public override void UpdateState(EnemyAI enemy)
        {
            Debug.Log("Trapped Update");
        }
    }

}
