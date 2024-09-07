using UnityEngine;

public class MimicEnemy : EnemyBase
{
    public GameObject disguiseObject; // Prefab for the disguise object
    public float spawnHeight = 2f; // How far above the enemy the disguise spawns

    private GameObject currentDisguise;

    private SleepState sleepState = new SleepState();

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
        Debug.Log(currentState);
    }

    // Method to instantiate the disguise object with a Rigidbody and spawn above the enemy
    public void TransformIntoDisguise(GameObject disguisePrefab)
    {
        if (currentDisguise != null)
        {
            GameObject.Destroy(currentDisguise); // Destroy the current disguise if it already exists
        }

        Debug.Log("Transforming into disguise");

        Vector3 spawnPosition = transform.position + Vector3.up * spawnHeight;
        currentDisguise = Instantiate(disguisePrefab, spawnPosition, Quaternion.identity);
        currentDisguise.SetActive(true);
        currentDisguise.transform.localScale = transform.localScale;

        Rigidbody rb = currentDisguise.AddComponent<Rigidbody>();
        rb.useGravity = true;
        rb.mass = 1f; // Adjust the mass as necessary for physics

        currentDisguise.transform.SetParent(null);
    }

    private class SleepState : EnemyBase.EnemyState
    {
        public override void EnterState(EnemyBase enemy)
        {
            MimicEnemy mimic = (MimicEnemy)enemy;
            Debug.Log("Entering Sleep State");

            // Transform into the given disguise (passed from the inspector or game logic)
            mimic.TransformIntoDisguise(mimic.disguiseObject);
            mimic.SetVisibility(false);
            mimic.SetCollision(false);
        }

        public override void UpdateState(EnemyBase enemy)
        {
            // Logic can be added here if needed

        }
    }
}
