using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy2 : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer, whatIsObstacle;

    // Enemy Attributes
    public float activateRange = 5f;
    public float deactivateRange = 20f;
    public bool playerInActivateRange = false;
    public float sightRange = 10f;
    
    public float enemySpeed = 5f;
    public float enemyAcceleration = 8f;
    public float enemyReach = 2f;
    public bool isTrapped = false;

    // Player vision attributes
    public float fieldOfViewAngle = 90f;  // Player's field of view in degrees

    private EnemyState currentState;
    private SleepState sleepState = new SleepState();
    private ChaseState chaseState = new ChaseState();
    private FreezeState freezeState = new FreezeState();

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = enemySpeed;
        agent.acceleration = enemyAcceleration;

        // Find the player
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        // Check if player is null
        if (player == null)
        {
            Debug.LogError("Player not found!");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (player != null)
        {
            currentState = sleepState;
            currentState.EnterState(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            currentState.UpdateState(this);

            // Check if the player is within the activation range
            playerInActivateRange = Physics.CheckSphere(transform.position, activateRange, whatIsPlayer);
            Debug.Log(currentState);
        }
        
    }

    public void SetIsTrapped(bool istrapped)
    {
        isTrapped = istrapped;
        if (isTrapped)
        {
            agent.speed = enemySpeed * 0.1f;
        }
        else
        {
            agent.speed = enemySpeed;
        }
    }

    public void TransitionToState(EnemyState state)
    {
        currentState = state;
        currentState.EnterState(this);
    }

    public abstract class EnemyState
    {
        public abstract void EnterState(Enemy2 enemy);
        public abstract void UpdateState(Enemy2 enemy);
    }

    private class SleepState : EnemyState
    {
        public override void EnterState(Enemy2 enemy)
        {
            Debug.Log("Entering Sleep State");
            Vector3 hideSpot = FindBestCover(enemy);
            if (hideSpot != enemy.transform.position)
            {
                enemy.agent.SetDestination(hideSpot);
                enemy.agent.isStopped = false;
            }
            else
            {
                enemy.agent.isStopped = true;
                Debug.Log("No good hiding spot found.");
            }
        }

        public override void UpdateState(Enemy2 enemy)
        {
            if (enemy.agent.remainingDistance < 0.5f)
            {
                enemy.agent.isStopped = true;
            }

            // Check if the player is in activation range
            enemy.playerInActivateRange = Physics.CheckSphere(enemy.transform.position, enemy.activateRange, enemy.whatIsPlayer);

            if (enemy.playerInActivateRange)
            {
                enemy.TransitionToState(enemy.chaseState);
            }

        }

        private Vector3 FindBestCover(Enemy2 enemy)
        {
            Collider[] obstacles = Physics.OverlapSphere(enemy.transform.position, enemy.sightRange, enemy.whatIsObstacle);
            Vector3 bestHidingSpot = enemy.transform.position;
            float bestScore = Mathf.Infinity; // Lower score is better

            foreach (var obstacle in obstacles)
            {
                Vector3 potentialHidingSpot = GetBehindObstacle(obstacle, enemy);
                float exposure = CalculateExposure(potentialHidingSpot, enemy);
                float distanceToObstacle = Vector3.Distance(potentialHidingSpot, obstacle.transform.position);

                // Calculate a score based on exposure and distance to the obstacle
                float score = exposure + distanceToObstacle;  // You can tweak this formula

                if (score < bestScore)
                {
                    bestHidingSpot = potentialHidingSpot;
                    bestScore = score;
                }
            }

            return bestHidingSpot;
        }

        private Vector3 GetBehindObstacle(Collider obstacle, Enemy2 enemy)
        {
            Vector3 directionAwayFromCenter = (enemy.transform.position - obstacle.transform.position).normalized;
            return obstacle.transform.position + directionAwayFromCenter * 2f;
        }

        private float CalculateExposure(Vector3 hideSpot, Enemy2 enemy)
        {
            float exposure = 0f;
            int numberOfRaycasts = 12;  // Higher number for more accuracy
            float angleStep = 360f / numberOfRaycasts;

            for (int i = 0; i < numberOfRaycasts; i++)
            {
                float angle = i * angleStep;
                Vector3 direction = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)).normalized;

                if (!Physics.Raycast(hideSpot, direction, enemy.sightRange, enemy.whatIsObstacle))
                {
                    exposure += 1f;  // Increment exposure if no obstacles are in this direction
                }
            }

            return exposure;  // Lower exposure values indicate better cover
        }

    }

    private class ChaseState : EnemyState
    {
        public override void EnterState(Enemy2 enemy)
        {
            enemy.agent.isStopped = false;  // Ensure the agent starts moving again if stopped
        }

        public override void UpdateState(Enemy2 enemy)
        {
            if (enemy.player == null) return; // Ensure player is valid

            float distanceToPlayer = Vector3.Distance(enemy.transform.position, enemy.player.position);
            Debug.Log(distanceToPlayer);

            if (distanceToPlayer > enemy.enemyReach)
            {
                enemy.agent.isStopped = false;
                enemy.agent.SetDestination(enemy.player.position);
            }
            else
            {
                enemy.agent.isStopped = true;
                enemy.agent.SetDestination(enemy.transform.position);  // Stop the agent at the current position
            }

            if (!IsEnemyInPlayerVision(enemy))  // Check if the player can see the enemy
            {
                enemy.TransitionToState(enemy.freezeState);
            }
            else if (distanceToPlayer > enemy.deactivateRange)  // Check if the player is outside the deactivation range
            {
                enemy.TransitionToState(enemy.sleepState);
            }
        } 

        private bool IsEnemyInPlayerVision(Enemy2 enemy)
        {
            // Get the vector from player to enemy
            Vector3 directionToEnemy = (enemy.transform.position - enemy.player.position).normalized;

            // Get the player's forward direction
            Vector3 playerForward = enemy.player.forward;

            // Calculate the dot product between player's forward direction and direction to enemy
            float dotProduct = Vector3.Dot(playerForward, directionToEnemy);

            // Calculate the angle between the two vectors
            float angleToEnemy = Mathf.Acos(dotProduct) * Mathf.Rad2Deg;

            // Check if the angle is within the player's field of view
            return angleToEnemy < enemy.fieldOfViewAngle / 2f;
        }
    }

    private class FreezeState : EnemyState
    {
        public override void EnterState(Enemy2 enemy)
        {
            Debug.Log("Entering Freeze State");
            enemy.agent.isStopped = true;
        }

        public override void UpdateState(Enemy2 enemy)
        {
            // Check if the player can not see the enemy
            if (!IsEnemyInPlayerVision(enemy))
            {
                enemy.agent.isStopped = true;  // Stay frozen if the player sees the enemy
            }
            else
            {
                enemy.TransitionToState(enemy.chaseState);  // Resume chasing if the player cannot see the enemy
            }
        }

        private bool IsEnemyInPlayerVision(Enemy2 enemy)
        {
            // Get the vector from player to enemy
            Vector3 directionToEnemy = (enemy.transform.position - enemy.player.position).normalized;

            // Get the player's forward direction
            Vector3 playerForward = enemy.player.forward;

            // Calculate the dot product between player's forward direction and direction to enemy
            float dotProduct = Vector3.Dot(playerForward, directionToEnemy);

            // Calculate the angle between the two vectors
            float angleToEnemy = Mathf.Acos(dotProduct) * Mathf.Rad2Deg;

            // Check if the angle is within the player's field of view
            return angleToEnemy < enemy.fieldOfViewAngle / 2f;
        }
    }
}
