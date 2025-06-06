using System.Collections.Generic;
using UnityEngine;

namespace EnvSpawnSystem
{
    public class EnvironementPiece : MonoBehaviour
    {
        [Header("Piece Structure")]
        [Tooltip("The point where this piece begins. Should align with the previous piece's End Point.")]
        public Transform entryPoint;

        [Tooltip("The point where the next piece should be spawned.")]
        public Transform endPoint;

        // --- NEW: SEPARATE SPAWNING LOGIC ---

        [Header("Obstacle Spawning")]
        [Tooltip("A list of all possible spawn locations for obstacles.")]
        [SerializeField]
        private List<Transform> obstacleSpawnPoints = new List<Transform>();

        [Tooltip("A list of obstacle prefabs to choose from.")] [SerializeField]
        private List<GameObject> obstaclePrefabs = new List<GameObject>();

        [Tooltip("The chance (from 0 to 1) that obstacles will spawn on this piece.")] [Range(0f, 1f)] [SerializeField]
        private float obstacleSpawnChance = 0.75f;

        [SerializeField] private int minObstaclesToSpawn = 1;
        [SerializeField] private int maxObstaclesToSpawn = 2;

        [Header("Booster Spawning")] [Tooltip("A list of all possible spawn locations for boosters.")] [SerializeField]
        private List<Transform> boosterSpawnPoints = new List<Transform>();

        [Tooltip("A list of booster prefabs to choose from.")] [SerializeField]
        private List<GameObject> boosterPrefabs = new List<GameObject>();

        [Tooltip("The chance (from 0 to 1) that boosters will spawn on this piece.")] [Range(0f, 1f)] [SerializeField]
        private float boosterSpawnChance = 0.25f;

        [SerializeField] private int minBoostersToSpawn = 0;
        [SerializeField] private int maxBoostersToSpawn = 1;


        void Start()
        {
            // Add the mover component if it doesn't exist
            if (GetComponent<WorldMover>() == null)
            {
                gameObject.AddComponent<WorldMover>();
            }

            // --- Call the spawning logic for both types ---
            ProcessSpawning(obstacleSpawnPoints, obstaclePrefabs, obstacleSpawnChance, minObstaclesToSpawn,
                maxObstaclesToSpawn);
            ProcessSpawning(boosterSpawnPoints, boosterPrefabs, boosterSpawnChance, minBoostersToSpawn,
                maxBoostersToSpawn);
        }

        void Update()
        {
            // Despawn logic remains the same
            if (transform.position.x < EnvironmentSpawner.Instance.DespawnXPosition)
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// A generic method to handle spawning items based on a chance.
        /// </summary>
        /// <param name="spawnPoints">The list of possible locations to spawn at.</param>
        /// <param name="prefabs">The list of prefabs to choose from.</param>
        /// <param name="chance">The probability (0-1) that any spawning will occur.</param>
        /// <param name="minToSpawn">The minimum number of items to spawn if the chance check succeeds.</param>
        /// <param name="maxToSpawn">The maximum number of items to spawn if the chance check succeeds.</param>
        private void ProcessSpawning(List<Transform> spawnPoints, List<GameObject> prefabs, float chance,
            int minToSpawn, int maxToSpawn)
        {
            // 1. First, check if spawning should happen at all based on the chance.
            // Random.value returns a float between 0.0 and 1.0.
            if (Random.value > chance)
            {
                return; // The chance failed, so we spawn nothing.
            }

            // 2. Validate that we have points and prefabs to work with.
            if (spawnPoints.Count == 0 || prefabs.Count == 0) return;

            // 3. Create a copy of the spawn points list so we can safely remove items from it.
            List<Transform> availableSpawnPoints = new List<Transform>(spawnPoints);

            // 4. Determine how many items to spawn.
            int itemsToSpawn = Random.Range(minToSpawn, maxToSpawn + 1);

            // 5. Loop and spawn the items.
            for (int i = 0; i < itemsToSpawn; i++)
            {
                // Safety check: if we've run out of available points, stop.
                if (availableSpawnPoints.Count == 0) break;

                // Pick a random spawn point from the available list
                int pointIndex = Random.Range(0, availableSpawnPoints.Count);
                Transform spawnPoint = availableSpawnPoints[pointIndex];

                // Pick a random prefab to spawn
                int prefabIndex = Random.Range(0, prefabs.Count);
                GameObject prefabToSpawn = prefabs[prefabIndex];

                // Instantiate the item and parent it to the spawn point
                Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation, spawnPoint);

                // Remove the used spawn point from the list to prevent duplicates
                availableSpawnPoints.RemoveAt(pointIndex);
            }
        }

        // Updated Gizmos to show different colors for obstacle and booster spawn points
        private void OnDrawGizmos()
        {
            // Entry and End points
            if (entryPoint != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(entryPoint.position, 0.5f);
            }

            if (endPoint != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(endPoint.position, 0.5f);
            }

            if (entryPoint != null && endPoint != null)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(entryPoint.position, endPoint.position);
            }

            // Obstacle Spawn Points
            Gizmos.color = new Color(1f, 0.5f, 0f); // Orange for obstacles
            foreach (var point in obstacleSpawnPoints)
            {
                if (point != null) Gizmos.DrawSphere(point.position, 0.3f);
            }

            // Booster Spawn Points
            Gizmos.color = Color.yellow; // Yellow for boosters
            foreach (var point in boosterSpawnPoints)
            {
                if (point != null) Gizmos.DrawWireSphere(point.position, 0.4f);
            }
        }
    }
}