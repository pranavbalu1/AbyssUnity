using UnityEngine;

public class Enemy2 : EnemyBase
{
    public float activateRange = 5f;
    public float deactivateRange = 20f;
    public bool playerInActivateRange = false;
    public float sightRange = 10f;
    public float fieldOfViewAngle = 90f;

    private SleepState sleepState = new SleepState();
    private ChaseState chaseState = new ChaseState();
    private FreezeState freezeState = new FreezeState();

    private void Start()
    {
        if (player != null)
        {
            currentState = sleepState;
            currentState.EnterState(this);
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
            Enemy2 enemy2 = (Enemy2)enemy;
            Debug.Log("Entering Sleep State");
            Vector3 hideSpot = FindBestCover(enemy2);
            if (hideSpot != enemy2.transform.position)
            {
                enemy2.agent.SetDestination(hideSpot);
                enemy2.agent.isStopped = false;
            }
            else
            {
                enemy2.agent.isStopped = true;
                Debug.Log("No good hiding spot found.");
            }
        }

        public override void UpdateState(EnemyBase enemy)
        {
            Enemy2 enemy2 = (Enemy2)enemy;

            if (enemy2.agent.remainingDistance < 0.5f)
            {
                enemy2.agent.isStopped = true;
            }

            enemy2.playerInActivateRange = Physics.CheckSphere(enemy2.transform.position, enemy2.activateRange, enemy2.whatIsPlayer);

            if (enemy2.playerInActivateRange)
            {
                enemy2.TransitionToState(enemy2.chaseState);
            }
        }

        private Vector3 FindBestCover(Enemy2 enemy)
        {
            Collider[] obstacles = Physics.OverlapSphere(enemy.transform.position, enemy.sightRange, enemy.whatIsObstacle);
            Vector3 bestHidingSpot = enemy.transform.position;
            float bestScore = Mathf.Infinity;

            foreach (var obstacle in obstacles)
            {
                Vector3 potentialHidingSpot = GetBehindObstacle(obstacle, enemy);
                float exposure = CalculateExposure(potentialHidingSpot, enemy);
                float distanceToObstacle = Vector3.Distance(potentialHidingSpot, obstacle.transform.position);
                float score = exposure + distanceToObstacle;

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
            int numberOfRaycasts = 12;
            float angleStep = 360f / numberOfRaycasts;

            for (int i = 0; i < numberOfRaycasts; i++)
            {
                float angle = i * angleStep;
                Vector3 direction = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)).normalized;

                if (!Physics.Raycast(hideSpot, direction, enemy.sightRange, enemy.whatIsObstacle))
                {
                    exposure += 1f;
                }
            }

            return exposure;
        }
    }

    private class ChaseState : EnemyBase.EnemyState
    {
        public override void EnterState(EnemyBase enemy)
        {
            Enemy2 enemy2 = (Enemy2)enemy;
            enemy2.agent.isStopped = false;
        }

        public override void UpdateState(EnemyBase enemy)
        {
            Enemy2 enemy2 = (Enemy2)enemy;

            if (enemy2.player == null) return;

            float distanceToPlayer = Vector3.Distance(enemy2.transform.position, enemy2.player.position);
            Debug.Log(distanceToPlayer);

            if (distanceToPlayer > enemy2.enemyReach)
            {
                enemy2.agent.isStopped = false;
                enemy2.agent.SetDestination(enemy2.player.position);
            }
            else
            {
                enemy2.agent.isStopped = true;
                enemy2.agent.SetDestination(enemy2.transform.position);
            }

            if (!IsEnemyInPlayerVision(enemy2))
            {
                enemy2.TransitionToState(enemy2.freezeState);
            }
            else if (distanceToPlayer > enemy2.deactivateRange)
            {
                enemy2.TransitionToState(enemy2.sleepState);
            }
        }

        private bool IsEnemyInPlayerVision(Enemy2 enemy)
        {
            Vector3 directionToEnemy = (enemy.transform.position - enemy.player.position).normalized;
            Vector3 playerForward = enemy.player.forward;
            float dotProduct = Vector3.Dot(playerForward, directionToEnemy);
            float angleToEnemy = Mathf.Acos(dotProduct) * Mathf.Rad2Deg;
            return angleToEnemy < enemy.fieldOfViewAngle / 2f;
        }
    }

    private class FreezeState : EnemyBase.EnemyState
    {
        public override void EnterState(EnemyBase enemy)
        {
            Enemy2 enemy2 = (Enemy2)enemy;
            Debug.Log("Entering Freeze State");
            enemy2.agent.isStopped = true;
        }

        public override void UpdateState(EnemyBase enemy)
        {
            Enemy2 enemy2 = (Enemy2)enemy;

            if (!IsEnemyInPlayerVision(enemy2))
            {
                enemy2.agent.isStopped = true;
            }
            else
            {
                enemy2.TransitionToState(enemy2.chaseState);
            }
        }

        private bool IsEnemyInPlayerVision(Enemy2 enemy)
        {
            Vector3 directionToEnemy = (enemy.transform.position - enemy.player.position).normalized;
            Vector3 playerForward = enemy.player.forward;
            float dotProduct = Vector3.Dot(playerForward, directionToEnemy);
            float angleToEnemy = Mathf.Acos(dotProduct) * Mathf.Rad2Deg;
            return angleToEnemy < enemy.fieldOfViewAngle / 2f;
        }
    }
}
