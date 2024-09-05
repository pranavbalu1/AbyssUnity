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
    public float activateRange = 10f;
    public bool playerInActivateRange = false;
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
        player = GameObject.Find("Player")?.transform;

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

    public class SleepState : EnemyState
    {
        public override void EnterState(Enemy2 enemy)
        {
            Debug.Log("Entering Sleep State");
        }

        public override void UpdateState(Enemy2 enemy)
        {
            // Check if the player is in activation range
            enemy.playerInActivateRange = Physics.CheckSphere(enemy.transform.position, enemy.activateRange, enemy.whatIsPlayer);

            if (enemy.playerInActivateRange)
            {
                enemy.TransitionToState(enemy.freezeState);
            }
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

            if (distanceToPlayer > enemy.enemyReach)
            {
                enemy.agent.SetDestination(enemy.player.position);
            }
            else
            {
                enemy.agent.isStopped = true;
            }

            // Check if the player can see the enemy
            if (IsEnemyInPlayerVision(enemy))
            {
                enemy.TransitionToState(enemy.freezeState);
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
            // Check if the player can see the enemy
            if (IsEnemyInPlayerVision(enemy))
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
