using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy2 : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public Camera playerCamera;  // Single player's camera
    public LayerMask whatIsGround, whatIsPlayer, whatIsObstacle;

    // Enemy Attributes
    public float activateRange = 10f;
    public bool playerInActivateRange = false;
    public float enemySpeed = 5f;
    public float enemyAcceleration = 8f;
    public float enemyReach = 2f;
    public bool isTrapped = false;

    private EnemyState currentState;
    private SleepState sleepState = new SleepState();
    private ChaseState chaseState = new ChaseState();
    private FreezeState freezeState = new FreezeState();

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = enemySpeed;
        agent.acceleration = enemyAcceleration;

        // Find the player and their camera
        player = GameObject.Find("Player").transform;
        playerCamera = Camera.main;  // Assuming the main camera is attached to the player
    }

    // Start is called before the first frame update
    void Start()
    {
        currentState = sleepState;
        currentState.EnterState(this);
    }

    // Update is called once per frame
    void Update()
    {
        currentState.UpdateState(this);

        // Check if the player is within the activation range
        playerInActivateRange = Physics.CheckSphere(transform.position, activateRange, whatIsPlayer);
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

    // Chase state class
    private class ChaseState : EnemyState
    {
        public override void EnterState(Enemy2 enemy)
        {
            enemy.agent.isStopped = false;  // Ensure the agent starts moving again if stopped
        }

        public override void UpdateState(Enemy2 enemy)
        {
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
            if (IsEnemyOnScreen(enemy))
            {
                enemy.TransitionToState(enemy.freezeState);
            }
        }

        // Check if the enemy is on the player's screen
        private bool IsEnemyOnScreen(Enemy2 enemy)
        {
            Vector3 screenPoint = enemy.playerCamera.WorldToViewportPoint(enemy.transform.position);
            return screenPoint.x >= 0 && screenPoint.x <= 1 && screenPoint.y >= 0 && screenPoint.y <= 1 && screenPoint.z > 0;
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
            if (IsEnemyOnScreen(enemy))
            {
                enemy.agent.isStopped = true;  // Stay frozen if the player sees the enemy
            }
            else
            {
                enemy.TransitionToState(enemy.chaseState);  // Resume chasing if the player cannot see the enemy
            }
        }

        // Check if the enemy is on the player's screen
        private bool IsEnemyOnScreen(Enemy2 enemy)
        {
            Vector3 screenPoint = enemy.playerCamera.WorldToViewportPoint(enemy.transform.position);
            return screenPoint.x >= 0 && screenPoint.x <= 1 && screenPoint.y >= 0 && screenPoint.y <= 1 && screenPoint.z > 0;
        }
    }
}
