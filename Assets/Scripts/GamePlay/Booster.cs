// Booster.cs - Attach to Booster prefabs
using UnityEngine;

public class Booster : MonoBehaviour
{
    [Header("Boost Settings")]
    [Tooltip("The flat amount of speed to add to the game's current speed.")]
    [SerializeField] private float speedBonus = 5f;
    
    [Tooltip("How long the speed boost and invincibility will last, in seconds.")]
    [SerializeField] private float boostDuration = 5f;

    // OnTriggerEnter2D is used for non-physics collisions (triggers)
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object that entered the trigger has the "Player" tag
        if (other.CompareTag("Player"))
        {
            // Get the player's Health component
            Health playerHealth = other.GetComponent<Health>();
            if (playerHealth != null)
            {
                // Grant invincibility by calling the function on the Health script
                playerHealth.SetInvincible(true, boostDuration);
            }

            // Tell the GameManager to apply the speed boost
            GameManager.Instance.ApplyTemporarySpeedBoost(speedBonus, boostDuration);

            // Optional: Play a sound or show a particle effect here

            // Destroy the booster so it can't be collected again
            Destroy(gameObject);
        }
    }
}