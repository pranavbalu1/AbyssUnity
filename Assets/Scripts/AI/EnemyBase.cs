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

        Vector3 directionToPlayer = player.position - transform.position;

        Debug.DrawRay(transform.position, directionToPlayer.normalized * directionToPlayer.magnitude, Color.red);

        // Use a SphereCast for better consistency (adjust the radius as needed).
        float sphereRadius = 0.2f; // Adjust the radius for better collision detection
        if (Physics.SphereCast(transform.position, sphereRadius, directionToPlayer.normalized, out RaycastHit hit, directionToPlayer.magnitude))
        {
            if (hit.transform == player)
            {
                return false; // No obstruction if we hit the player directly.
            }
        }

        Debug.Log("No hit detected, player not obstructed.");
        return true;
    }


    // Base state class for shared state behavior
    public abstract class EnemyState
    {
        public abstract void EnterState(EnemyBase enemy);
        public abstract void UpdateState(EnemyBase enemy);
    }

  

}
