using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Serialization; // Needed for System.Action

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    [Tooltip("The maximum health value.")]
    [SerializeField]
    [Min(1)] // Ensure max health is at least 1
    private int maxHealth = 2;

    [Tooltip("Check this box to make the object invincible (cannot die).")]
    [SerializeField]
    private bool isInvincible = false;

    // --- Private State ---
    [Tooltip("Current health value (visible for debugging).")]
    [SerializeField] // Show in inspector for debugging, but controlled by code
    private int currentHealth;

    private bool isDead = false;

    [Header("SFXs")]
    [SerializeField] private AudioClip hitObjectSFX;
    [FormerlySerializedAs("hitCharacterSFX")] [SerializeField] private AudioClip hitCharacterSFX_male;
    [FormerlySerializedAs("hitCharacterSFX")] [SerializeField] private AudioClip hitCharacterSFX_female;

    // --- Events ---
    // Event triggered when health changes (passes current health, max health)
    public event Action<int, int> OnHealthChanged;
    // Event triggered when health reaches zero
    public event Action OnDied;

    // --- Properties ---
    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsDead => isDead;
    public bool IsInvincible => isInvincible; // Public getter for invincibility status

    private Coroutine activeSpeedBoostCoroutine;
    
    void Awake()
    {
        // Initialize health on awake
        currentHealth = maxHealth;
    }

    void Start()
    {
        // Trigger initial health update for UI etc.
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    /// <summary>
    /// Reduces health by the specified amount.
    /// </summary>
    /// <param name="amount">The amount of damage to take.</param>
    public void TakeDamage(int amount)
    {
        if (isDead || amount <= 0)
        {
            // Cannot take damage if already dead or damage amount is zero/negative
            return;
        }

        //SpeedGameManager.Instance.DamagePlayer();
        SpeedGameAudioManager.Instance.PlaySFXNoEffect(hitObjectSFX);

        // If invincible, prevent health from dropping below 1
        if (isInvincible && currentHealth - amount < 1)
        {
            // Only apply damage if current health is already greater than 1
            if (currentHealth > 1)
            {
                int damageToApply = currentHealth - 1; // Damage needed to reach exactly 1 health
                currentHealth = 1;
                Debug.Log($"{gameObject.name} took {damageToApply} damage (Invincible). Health: {currentHealth}/{maxHealth}");
                OnHealthChanged?.Invoke(currentHealth, maxHealth);
            }
            // If already at 1 health and invincible, take no damage
            return;
        }

        // Apply damage normally
        currentHealth -= amount;
        //Debug.Log($"{gameObject.name} took {amount} damage. Health: {currentHealth}/{maxHealth}");


        // Clamp health to minimum 0
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        // Trigger health changed event
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        // Check for death
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Increases health by the specified amount, up to max health.
    /// </summary>
    /// <param name="amount">The amount of health to restore.</param>
    public void Heal(int amount)
    {
        if (isDead || amount <= 0 || currentHealth >= maxHealth)
        {
            // Cannot heal if dead, amount is zero/negative, or already at max health
            return;
        }

        currentHealth += amount;

        // Clamp health to maximum
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        Debug.Log($"{gameObject.name} healed {amount}. Health: {currentHealth}/{maxHealth}");

        // Trigger health changed event
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    /// <summary>
    /// Handles the death logic.
    /// </summary>
    private void Die()
    {
        if (isDead) return; // Prevent multiple death triggers

        isDead = true;
        //Debug.Log($"{gameObject.name} has died!");

        // Trigger the death event for other scripts to react
        OnDied?.Invoke();

        // --- Add specific death behaviors here ---
        // For example:
        // - Disable player movement scripts
        // - Play death animation
        // - Show game over screen after a delay

        GameManager.Instance.SetState(GameManager.GameState.GameOver);
    }

    /// <summary>
    /// Sets the invincibility state.
    /// Useful for temporary power-ups.
    /// </summary>
    public void SetInvincible(bool invincible, float duration)
    {
        isInvincible = invincible;
        Debug.Log($"{gameObject.name} Invincibility set to: {isInvincible}");

        activeSpeedBoostCoroutine = StartCoroutine(InvincibleBoostRoutine(invincible, duration));
    }

    private IEnumerator InvincibleBoostRoutine(bool invincible, float duration)
    {
        isInvincible = invincible;
        
        yield return new WaitForSeconds(duration);
        
        isInvincible = false;
        activeSpeedBoostCoroutine = null; // Mark the coroutine as finished
    }
    
    /// <summary>
    /// Instantly kills the object, bypassing invincibility. Useful for kill zones etc.
    /// </summary>
    public void ForceKill()
    {
        if (isDead) return;

        currentHealth = 0;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        Die();
    }
}