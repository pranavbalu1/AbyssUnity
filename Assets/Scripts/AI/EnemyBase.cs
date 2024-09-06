using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyBase : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer, whatIsObstacle;

    // Enemy Attributes
    public float enemySpeed = 5f;
    public float enemyAcceleration = 8f;
    public float enemyReach = 2f;
    public bool isTrapped = false;

    protected EnemyState currentState;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = enemySpeed;
        agent.acceleration = enemyAcceleration;

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
        {
            Debug.LogError("Player not found!");
        }
    }

    protected virtual void Update()
    {
        currentState?.UpdateState(this);
    }

    public void SetIsTrapped(bool istrapped)
    {
        isTrapped = istrapped;
        agent.speed = isTrapped ? enemySpeed * 0.1f : enemySpeed;
    }

    public void TransitionToState(EnemyState state)
    {
        currentState = state;
        currentState.EnterState(this);
    }

    public bool IsPlayerObstructed()
    {
        return false;
    }

    // Base state class for shared state behavior
    public abstract class EnemyState
    {
        public abstract void EnterState(EnemyBase enemy);
        public abstract void UpdateState(EnemyBase enemy);
    }

  

}
