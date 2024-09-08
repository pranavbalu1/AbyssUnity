using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : EnemyBase
{
    public float sightRange = 20f;
    public bool playerInSightRange = false;

    // States specific to EnemyAI
    private PatrolState patrolState = new PatrolState();
    private ChaseState chaseState = new ChaseState();
    private HideState hideState = new HideState();

    private void Start()
    {
        currentState = patrolState;
        currentState.EnterState(this);
    }

    protected override void Update()
    {
        base.Update();

        // Check for sight range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);

        // Transition based on player visibility
        if (!playerInSightRange && currentState != patrolState)
        {
            TransitionToState(patrolState);
        }
        else if (playerInSightRange && currentState != chaseState)
        {
            TransitionToState(hideState);
        }
    }

    // State Classes inheriting from EnemyBase.EnemyState
    private class PatrolState : EnemyBase.EnemyState
    {
        public override void EnterState(EnemyBase enemy)
        {
            EnemyAI enemyAI = (EnemyAI)enemy;
            Debug.Log("Entering Patrol State");
        }

        public override void UpdateState(EnemyBase enemy)
        {
            EnemyAI enemyAI = (EnemyAI)enemy;

            if (enemyAI.agent.remainingDistance < 5f)
            {
                float x = Random.Range(-11, 11);
                float z = Random.Range(-11, 11);
                Vector3 position = new Vector3(x, 0f, z);
                enemyAI.agent.SetDestination(position);
            }
        }
    }

    private class HideState : EnemyBase.EnemyState
    {
        public override void EnterState(EnemyBase enemy)
        {
            EnemyAI enemyAI = (EnemyAI)enemy;
            Vector3 hideSpot = FindHidingSpot(enemyAI);

            if (hideSpot == enemyAI.transform.position)
            {
                enemyAI.TransitionToState(enemyAI.chaseState);
            }

            enemyAI.agent.SetDestination(hideSpot);
            enemyAI.agent.isStopped = false;
            Debug.Log($"Hiding at {hideSpot}");
        }

        public override void UpdateState(EnemyBase enemy)
        {
            EnemyAI enemyAI = (EnemyAI)enemy;

            if (enemyAI.agent.remainingDistance < 0.5f)
            {
                enemyAI.agent.isStopped = true;
            }
        }

        private Vector3 FindHidingSpot(EnemyAI enemyAI)
        {
            Collider[] obstacles = Physics.OverlapSphere(enemyAI.transform.position, enemyAI.sightRange, enemyAI.whatIsObstacle);
            Vector3 bestHidingSpot = enemyAI.transform.position;
            float bestHidingSpotDistance = Mathf.Infinity;

            foreach (var obstacle in obstacles)
            {
                Vector3 directionToPlayer = (enemyAI.player.position - obstacle.transform.position).normalized;
                Vector3 potentialHidingSpot = obstacle.transform.position - directionToPlayer * 2f;

                if (IsOccludedFromPlayer(potentialHidingSpot, enemyAI))
                {
                    float distanceToPlayer = Vector3.Distance(potentialHidingSpot, enemyAI.player.position);
                    if (distanceToPlayer < bestHidingSpotDistance)
                    {
                        bestHidingSpot = potentialHidingSpot;
                        bestHidingSpotDistance = distanceToPlayer;
                    }
                }
            }

            return bestHidingSpot;
        }

        private bool IsOccludedFromPlayer(Vector3 spot, EnemyAI enemyAI)
        {
            Vector3 directionToPlayer = enemyAI.player.position - spot;
            Ray ray = new Ray(spot, directionToPlayer);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, directionToPlayer.magnitude))
            {
                return hit.transform != enemyAI.player;
            }

            return false;

        }
    }

    private class ChaseState : EnemyBase.EnemyState
    {
        public override void EnterState(EnemyBase enemy)
        {
            EnemyAI enemyAI = (EnemyAI)enemy;
            enemyAI.agent.isStopped = false;
        }

        public override void UpdateState(EnemyBase enemy)
        {
            EnemyAI enemyAI = (EnemyAI)enemy;
            float distanceToPlayer = Vector3.Distance(enemyAI.transform.position, enemyAI.player.position);

            if (distanceToPlayer > enemyAI.enemyReach)
            {
                enemyAI.agent.isStopped = false;
                enemyAI.agent.SetDestination(enemyAI.player.position);
            }
            else
            {
                enemyAI.agent.isStopped = true;
                enemyAI.agent.SetDestination(enemyAI.transform.position);
            }
        }
    }
}
