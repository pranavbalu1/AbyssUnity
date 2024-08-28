using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform player;

    public LayerMask whatIsGround, whatIsPlayer;

    public float health = 10;  // Set a default health value


    //States
    public float sightRange = 20;
    public bool playerInSightRange = false;
    private float enemySpeed = 5;

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        //Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);

        if (!playerInSightRange)
            Patroling();
        else
            ChasePlayer();
    }

    private void Patroling()
    {
        if (agent.remainingDistance < 0.5f)
        {
            float x = Random.Range(-11, 11);
            float z = Random.Range(-11, 11);
            Vector3 position = new Vector3(x, 0f, z);
            agent.SetDestination(position);
            //Debug.Log($"Patrolling to {patrolPoints[currentPatrolPointIndex].position}");
        }
    }

    private void ChasePlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer > 4f)
        {
            agent.SetDestination(player.position);
            agent.speed = enemySpeed;
            Debug.Log($"Chasing Player {player.position}");
        }
        else
        {
            agent.isStopped = true;
            Debug.Log($"Chasing Player {player.position}. Slowing down.");
        }
    }

}
