/// <summary>
    /// Get force multiplier based on obstacle type - using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    // Define different types of obstacles
    public enum ObstacleType
    {
        FullBlock,      // Requires lane change
        JumpOver,       // Requires jump
        SlideUnder,     // Requires crouch/roll
        Passable        // E.g., a ramp, or maybe destructible - doesn't impede basic running
    }

    [Header("Obstacle Settings")]
    [Tooltip("Type of obstacle, determines how the player might interact.")]
    public ObstacleType type = ObstacleType.FullBlock;

    [Tooltip("How many damage points inflects on the player")]
    [SerializeField] private int damageAmount = 1;
    
    [Tooltip("Part of the Obstacle that playerCanWalkOn")]
    [SerializeField] private GameObject solidCollision;

    [Header("Physics Response")]
    [Tooltip("Force multiplier applied when hit by player")]
    [SerializeField] private float hitForceMultiplier = 500f;
    
    [Tooltip("Torque multiplier for rotational force when hit")]
    [SerializeField] private float torqueMultiplier = 100f;
    
    [Tooltip("Time before obstacle gets destroyed after being hit (0 = never destroy)")]
    [SerializeField] private float destroyDelayAfterHit = 5f;
    
    [Tooltip("Should this obstacle block path after being hit?")]
    [SerializeField] private bool blocksPathAfterHit = false;

    [Tooltip("Should the obstacle be destroyed immediately when hit?")]
    [SerializeField] private bool destroyOnHit = false;

    [Header("Associated Coin Spawning")]
    [Tooltip("List of Transforms defining the START (bottom) position of potential coin columns.")]
    [SerializeField] private List<Transform> coinColumnSpawnPoints;

    [Tooltip("How many coins to spawn vertically in each chosen column.")]
    [SerializeField] private int coinsPerColumn = 3;

    [Tooltip("Vertical distance between coins in a column.")]
    [SerializeField] private float coinSpacing = 0.6f;
    
    [SerializeField] private GameObject hitEffectPrefab;

    // --- Properties ---
    public int DamageAmount => damageAmount;

    // --- Runtime Variables ---
    private Rigidbody2D obstacleRigidbody;
    private bool hasBeenHit = false;
    private GameObject coinPrefabRef;
    private Collider[] allColliders;

    void Awake()
    {
        obstacleRigidbody = GetComponent<Rigidbody2D>();
        allColliders = GetComponents<Collider>();
        
        if (obstacleRigidbody == null)
        {
            Debug.LogError($"Obstacle {gameObject.name} is missing Rigidbody component! Physics response will not work.", this);
        }

        // Ensure rigidbody starts as kinematic for predictable placement
        if (obstacleRigidbody != null)
        {
            obstacleRigidbody.isKinematic = true;
        }
    }

    /// <summary>
    /// Called when player hits this obstacle - applies physics response
    /// </summary>
    /// <param name="playerTransform">Player's transform for calculating hit direction</param>
    /// <param name="playerVelocity">Player's current velocity</param>
    public void OnPlayerHit(Transform playerTransform, Vector3 playerVelocity)
    {
        if (hasBeenHit || obstacleRigidbody == null) return;

        hasBeenHit = true;
        // Immediate destruction if configured
        if (destroyOnHit)
        {
            Destroy(gameObject);
            return;
        }
/*
        // Calculate realistic hit direction using enhanced physics
        Vector3 hitDirection = CalculateRealisticHitDirection(playerTransform);
        Instantiate(hitEffectPrefab,playerTransform.position,Quaternion.identity );
        // Calculate force based on obstacle type
        float forceMultiplier = GetForceMultiplierByType();
        Vector3 force = hitDirection * hitForceMultiplier * forceMultiplier;

        // Add player velocity influence for more realistic physics
        force += playerVelocity * 0.3f;

        // Apply physics response
        ApplyPhysicsResponse(force);

        // Handle path blocking
        if (!blocksPathAfterHit)
        {
            DisableCollisionAfterHit();
        }

        // Schedule destruction if needed
        if (destroyDelayAfterHit > 0)
        {
            StartCoroutine(DestroyAfterDelay());
        }*/
    }

    /// <summary>
    /// Calculate realistic hit direction based on player approach angle
    /// </summary>
    private Vector3 CalculateRealisticHitDirection(Transform playerTransform)
    {
        // Calculate horizontal hit direction
        Vector3 hitDirection = (transform.position - playerTransform.position);
        hitDirection.y = 0; // Remove vertical component
        
        // If hit is too head-on, force more lateral movement
        if (hitDirection.magnitude < 0.8f)
        {
            // Create more dramatic left/right movement for head-on hits
            float leftOrRight = Random.value > 0.5f ? 1f : -1f;
            hitDirection = new Vector3(leftOrRight, 0, Random.Range(-0.1f, 0.3f));
        }
        
        hitDirection = hitDirection.normalized;

        // ENHANCED: Favor left/right movement over forward movement
        float randomX = Random.Range(-0.8f, 0.8f);  // Much stronger left/right
        float randomZ = Random.Range(-0.4f, 0.1f);  // Reduce forward movement
        hitDirection.x += randomX;
        hitDirection.z += randomZ;

        // Smaller, more realistic bounce
        hitDirection.y = Random.Range(0.05f, 0.08f);
        
        return hitDirection.normalized;
    }

    /// <summary>
    /// Apply physics force and torque to the obstacle
    /// </summary>
    private void ApplyPhysicsResponse(Vector3 force)
    {
        /*if (obstacleRigidbody == null) return;

        // Enable physics (disable kinematic)
        obstacleRigidbody.isKinematic = false;

        // Apply force
        obstacleRigidbody.AddForce(force, ForceMode.Impulse);

        // Apply random torque for realistic tumbling
        Vector3 randomTorque = new Vector3(
            Random.Range(-torqueMultiplier, torqueMultiplier),
            Random.Range(-torqueMultiplier, torqueMultiplier),
            Random.Range(-torqueMultiplier, torqueMultiplier)
        );
        obstacleRigidbody.AddTorque(randomTorque, ForceMode.Impulse);*/
    }

    /// <summary>
    /// Get force multiplier based on obstacle type
    /// </summary>
    private float GetForceMultiplierByType()
    {
        switch (type)
        {
            case ObstacleType.FullBlock:
                return 1.0f; // Standard force
            case ObstacleType.JumpOver:
                return 0.7f; // Lighter objects fly easier
            case ObstacleType.SlideUnder:
                return 1.3f; // Heavier, more resistance
            case ObstacleType.Passable:
                return 0.5f; // Very light response
            default:
                return 1.0f;
        }
    }

    /// <summary>
    /// Disable collision so obstacle doesn't interfere with player after being hit
    /// </summary>
    private void DisableCollisionAfterHit()
    {
        if (allColliders != null)
        {
            foreach (Collider col in allColliders)
            {
                col.enabled = false;
            }
        }
    }

    /// <summary>
    /// Destroy obstacle after specified delay
    /// </summary>
    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(destroyDelayAfterHit);
        
        if (gameObject != null)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Call this AFTER the obstacle is instantiated and positioned
    /// </summary>
    public void InitializeAndSpawnCoins(GameObject prefab)
    {
        if (prefab == null)
        {
            return;
        }
        coinPrefabRef = prefab;
        //SpawnAssociatedCoins();
    }

    /// <summary>
    /// Helper function to quickly check if this obstacle blocks a lane completely
    /// </summary>
    public bool IsHardBlocker()
    {
        return type == ObstacleType.FullBlock;
    }

    /// <summary>
    /// Helper function to check if it's passable without special action
    /// </summary>
    public bool IsPassableWithoutAction()
    {
        return type == ObstacleType.Passable;
    }

    // --- Coin Spawning Methods ---
/*
    private void SpawnAssociatedCoins()
    {
        if (coinPrefabRef == null || coinColumnSpawnPoints == null || coinColumnSpawnPoints.Count == 0)
        {
            return;
        }

        // Find all pattern points within this environment piece's hierarchy
        CoinPatternPoint[] patternPoints = GetComponentsInChildren<CoinPatternPoint>();

        if (patternPoints.Length == 0)
        {
            return;
        }

        if(DifficultyManager.Instance != null && Random.value <= DifficultyManager.Instance.GetCurrentCoinSpawnChance())
        {
            foreach (CoinPatternPoint point in patternPoints)
            {
                switch (point.patternType)
                {
                    case CoinPatternPoint.PatternType.Line:
                        SpawnLinePattern(point);
                        break;

                    case CoinPatternPoint.PatternType.Arc:
                        SpawnArcPattern(point);
                        break;

                    default:
                        Debug.LogWarning($"Unhandled PatternType: {point.patternType} on {point.gameObject.name}", point);
                        break;
                }
            }
        }
    }

    private void SpawnLinePattern(CoinPatternPoint point)
    {
        if (DifficultyManager.Instance.GetDefaultCoinsPerLine() <= 0) return;

        Transform pointTransform = point.transform;

        for (int i = 0; i < DifficultyManager.Instance.GetDefaultCoinsPerLine(); i++)
        {
            Vector3 spawnPos = pointTransform.position + pointTransform.forward * i * point.lineCoinSpacing;
            Instantiate(coinPrefabRef, spawnPos, pointTransform.rotation);
        }
    }

    private void SpawnArcPattern(CoinPatternPoint point)
    {
        if (DifficultyManager.Instance.GetDefaultCoinsPerArc() <= 0) return;

        Transform pointTransform = point.transform;

        if (DifficultyManager.Instance.GetDefaultCoinsPerArc() == 1)
        {
            Instantiate(coinPrefabRef, pointTransform.position, pointTransform.rotation);
            return;
        }

        float totalAngleRad = point.arcAngle * Mathf.Deg2Rad;
        float angleStepRad = totalAngleRad / (DifficultyManager.Instance.GetDefaultCoinsPerArc() - 1);
        float startAngleRad = -totalAngleRad / 2.0f;

        for (int i = 0; i < DifficultyManager.Instance.GetDefaultCoinsPerArc(); i++)
        {
            float currentAngleRad = startAngleRad + i * angleStepRad;
            Vector3 direction = Quaternion.AngleAxis(currentAngleRad * Mathf.Rad2Deg, pointTransform.right) * pointTransform.forward;
            Vector3 spawnPos = pointTransform.position + direction * point.arcRadius;
            Instantiate(coinPrefabRef, spawnPos, Quaternion.identity);
        }
    }
*/
    // --- Interface Implementations ---

    private void EnableSolidCollision()
    {
        if (solidCollision != null)
        {
            solidCollision.SetActive(true);
        }
    }

    private void DisableSolidCollision()
    {
        if (solidCollision != null)
        {
            solidCollision.SetActive(false);
        }
    }
}