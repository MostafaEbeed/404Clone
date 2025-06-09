using System.Collections.Generic;
using UnityEngine;

namespace EnvSpawnSystem
{
    public class EnvironmentSpawner : MonoBehaviour
    {
        public static EnvironmentSpawner Instance { get; private set; }

        [Header("Spawner Settings")]
        [Tooltip("The list of environment prefabs to spawn. They must have the EnvironmentPiece script.")]
        [SerializeField]
        private List<EnvironementPiece> environmentPrefabs;
        [SerializeField]
        private EnvironementPiece environmentFirstPiecePrefab;

        [Tooltip("The starting point where the very first piece will be spawned.")] [SerializeField]
        private Transform initialSpawnPoint;

        [Tooltip("How many pieces should be pre-spawned at the start of the game.")] [SerializeField]
        private int initialPiecesToSpawn = 3;

        [Header("Spawning Logic")]
        [Tooltip("How close the player needs to get to the end of the current track to trigger a new piece spawn.")]
        [SerializeField]
        private float playerTriggerDistance = 20f;

        [Header("Spawning Boundaries")] [Tooltip("The X position at which an old piece should be destroyed.")]
        public float DespawnXPosition = -30f;

        // --- NEW: A reference to the last piece that was spawned ---
        private EnvironementPiece lastSpawnedPiece;

        private float distanceToPlayer;
        
        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(gameObject);
            else Instance = this;
        }

        private void Start()
        {
            GameManager.OnStateChanged += HandleGameStateChange;
        }

        private void OnDestroy()
        {
            GameManager.OnStateChanged -= HandleGameStateChange;
        }

        // --- NEW: The Update method now holds the trigger logic ---
        private void Update()
        {
            // Only run spawn checks during gameplay and if we have a valid last piece and player
            if (GameManager.Instance.CurrentState != GameManager.GameState.Gameplay ||
                lastSpawnedPiece == null)
            {
                return;
            }

            // Check the distance between the player and the end point of the last piece
            distanceToPlayer =
                lastSpawnedPiece.endPoint.position.x - PlayerController.Instance.transform.position.x;

            // If the player is close enough, spawn the next piece
            if (distanceToPlayer < playerTriggerDistance)
            {
                SpawnNextPiece();
            }
        }

        private void HandleGameStateChange(GameManager.GameState newState)
        {
            if (newState == GameManager.GameState.Gameplay)
            {
                // Clear any old pieces if we are restarting a game
                foreach (var piece in FindObjectsOfType<EnvironementPiece>())
                {
                    Destroy(piece.gameObject);
                }

                // Spawn the first piece at the initial position
                SpawnFirstPiece();

                // Pre-spawn the next few pieces to create the starting track
                for (int i = 0; i < initialPiecesToSpawn - 1; i++)
                {
                    SpawnNextPiece();
                }
            }
        }

        private void SpawnFirstPiece()
        {
            EnvironementPiece piecePrefab = environmentFirstPiecePrefab;

            // Instantiate the first piece and update our reference
            lastSpawnedPiece = Instantiate(piecePrefab, initialSpawnPoint.position, Quaternion.identity);
        }

        // --- UPDATED: The SpawnNextPiece method now uses lastSpawnedPiece to determine where to spawn ---
        public void SpawnNextPiece()
        {
            if (environmentPrefabs.Count == 0 || lastSpawnedPiece == null)
            {
                Debug.LogError("Cannot spawn next piece: Prefab list is empty or lastSpawnedPiece is null.");
                return;
            }

            // 1. Pick a random piece from the list
            int randomIndex = Random.Range(0, environmentPrefabs.Count);
            EnvironementPiece piecePrefab = environmentPrefabs[randomIndex];

            // 2. Calculate the exact spawn position to stitch the pieces together seamlessly.
            // The new piece's entry point should be at the old piece's end point.
            Vector3 spawnPosition = lastSpawnedPiece.endPoint.position;
            Vector3 offset = spawnPosition - piecePrefab.entryPoint.position;

            // 3. Instantiate the piece at the calculated position.
            EnvironementPiece newPiece = Instantiate(piecePrefab, offset, Quaternion.identity);

            // 4. IMPORTANT: Update the reference to the last spawned piece.
            lastSpawnedPiece = newPiece;
        }
    }
}