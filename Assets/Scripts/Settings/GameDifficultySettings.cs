using UnityEngine;

// This attribute allows you to create instances of this ScriptableObject
// from the Assets > Create menu in the Unity Editor.
[CreateAssetMenu(fileName = "NewGameDifficulty", menuName = "Game/Difficulty Settings", order = 1)]
public class GameDifficultySettings : ScriptableObject
{
    [Header("Initial Spawn Chances (0.0 to 1.0)")]
    [Tooltip("Initial chance that a defined coin pattern point will actually spawn its coins.")]
    [Range(0f, 1f)] public float initialCoinSpawnChancePerPoint = 0.7f;

    [Tooltip("Initial chance that an obstacle will spawn at a potential spawn point on an environment piece (0, 1, or 2 obstacles per row based on this). This is a general guide, specific logic in EnvironmentPiece handles the row guarantees.")]
    [Range(0f, 1f)] public float initialObstacleSpawnChance = 0.6f; // This might need more nuanced application

    [Header("Power-Up Timing")]
    [Tooltip("Time in seconds between attempts to spawn a new power-up.")]
    [Min(1f)] public float timeToSpawnNewPowerUp = 15f;

    [Header("Coin Pattern Parameters")]
    [Tooltip("Default number of coins to spawn in a line pattern (can be overridden by CoinPatternPoint settings).")]
    [Min(1)] public int defaultCoinsPerLine = 5;
    [Tooltip("Default number of coins to spawn in an arc pattern (can be overridden by CoinPatternPoint settings).")]
    [Min(1)] public int defaultCoinsPerArc = 7;


    [Header("Difficulty Scaling")]
    [Tooltip("How much to increase the coin spawn chance per interval (added to current chance, capped at 1.0).")]
    [Range(0f, 0.1f)] public float coinSpawnChanceIncreaseRate = 0.01f;

    [Tooltip("How much to increase the general obstacle spawn chance per interval (added to current chance, capped at 1.0).")]
    [Range(0f, 0.1f)] public float obstacleSpawnChanceIncreaseRate = 0.005f;

    [Tooltip("Time interval in seconds at which the spawn rates are increased.")]
    [Min(1f)] public float timeIntervalToIncreaseRates = 30f;

    // --- Helper methods to get current values if you want to manage them dynamically ---
    // These are simple examples; true dynamic values would need to be stored elsewhere
    // and potentially modified by a game manager during runtime.
    // For now, these just return the ScriptableObject's set values.

    public float GetInitialCoinSpawnChance() => initialCoinSpawnChancePerPoint;
    public float GetInitialObstacleSpawnChance() => initialObstacleSpawnChance;
    // ... and so on for other values if needed.
}