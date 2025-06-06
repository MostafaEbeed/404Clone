using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager Instance { get; private set; }

    [Header("Difficulty")]
    [Tooltip("The active difficulty settings for the current game session.")]
    public GameDifficultySettings currentDifficulty; // Assign your created SO asset here in Inspector

    // --- Runtime dynamic values (managed by GameManager) ---
    private float runtimeCoinSpawnChance;
    private float runtimeObstacleSpawnChance;
    private float timeSinceLastRateIncrease = 0f;

    private float timeSinceLastBoosterSpawned;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        if (currentDifficulty == null)
        {
            Debug.LogError("GameManager: Current Difficulty Settings not assigned!", this);
            // Fallback or load a default if needed
            enabled = false;
            return;
        }

        // Initialize runtime values from the ScriptableObject
        runtimeCoinSpawnChance = currentDifficulty.initialCoinSpawnChancePerPoint;
        runtimeObstacleSpawnChance = currentDifficulty.initialObstacleSpawnChance;
    }

    void Update()
    {
        if (currentDifficulty == null) return;

        // Handle difficulty scaling over time
        if (GameManager.Instance.CurrentState == GameManager.GameState.Gameplay) // Only during gameplay
        {
            timeSinceLastRateIncrease += Time.deltaTime;
            if (timeSinceLastRateIncrease >= currentDifficulty.timeIntervalToIncreaseRates)
            {
                timeSinceLastRateIncrease = 0f; // Reset timer

                // Increase coin spawn chance
                runtimeCoinSpawnChance += currentDifficulty.coinSpawnChanceIncreaseRate;
                runtimeCoinSpawnChance = Mathf.Clamp(runtimeCoinSpawnChance, 0f, 1f); // Cap at 100%

                // Increase obstacle spawn chance
                runtimeObstacleSpawnChance += currentDifficulty.obstacleSpawnChanceIncreaseRate;
                runtimeObstacleSpawnChance = Mathf.Clamp(runtimeObstacleSpawnChance, 0f, 1f);

                Debug.Log($"Difficulty Scaled: CoinChance={runtimeCoinSpawnChance:P1}, ObstacleChance={runtimeObstacleSpawnChance:P1}");
            }

            timeSinceLastBoosterSpawned += Time.deltaTime;
        }
    }

    // --- Public accessors for runtime values ---
    public float GetCurrentCoinSpawnChance() => runtimeCoinSpawnChance;
    public float GetCurrentObstacleSpawnChance() => runtimeObstacleSpawnChance;
    public float GetTimeToSpawnPowerUp() => currentDifficulty.timeToSpawnNewPowerUp;
    public float SetTimeToSpawnPowerUp() => timeSinceLastBoosterSpawned = 0;
    public bool CanSpawnPowerUp() => timeSinceLastBoosterSpawned >= currentDifficulty.timeToSpawnNewPowerUp;
    public int GetDefaultCoinsPerLine() => currentDifficulty.defaultCoinsPerLine;
    public int GetDefaultCoinsPerArc() => currentDifficulty.defaultCoinsPerArc;
}
