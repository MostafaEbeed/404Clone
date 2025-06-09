// Booster.cs - Attach to Booster prefabs
using UnityEngine;

public class Booster : MonoBehaviour, IPlayerStateListener
{
    [Header("Boost Settings")]
    [Tooltip("The flat amount of speed to add to the game's current speed.")]
    [SerializeField] private float speedBonus = 5f;
    
    [Tooltip("How long the speed boost and invincibility will last, in seconds.")]
    [SerializeField] private float boostDuration = 5f;
    
    [SerializeField] private AudioClip boostSound;

    // OnTriggerEnter2D is used for non-physics collisions (triggers)
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object that entered the trigger has the "Player" tag
        if (other.CompareTag("Player"))
        {
            // Get the player's Health component
            /*Health playerHealth = other.GetComponent<Health>();
            if (playerHealth != null)
            {
                // Grant invincibility by calling the function on the Health script
                playerHealth.SetInvincible(true, boostDuration);
            }*/

            BoosterHandler.Instance.ActivateBooster(speedBonus, boostDuration);
            
            // Tell the GameManager to apply the speed boost
            GameManager.Instance.ApplyTemporarySpeedBoost(speedBonus, boostDuration);

            SpeedGameAudioManager.Instance.PlaySFX(boostSound);
            
            // Optional: Play a sound or show a particle effect here

            // Destroy the booster so it can't be collected again
            Destroy(gameObject);
        }
    }

    public void OnPlayerStateChange(GameManager.PlayerState playerState)
    {
        if(playerState == GameManager.PlayerState.Boosted)
            Destroy(gameObject);
    }
}