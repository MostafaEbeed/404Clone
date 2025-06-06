// Obstacle.cs - Attach to Obstacle prefabs
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [Tooltip("The amount of damage this obstacle deals to the player.")]
    [SerializeField] private int damageAmount = 1;

    // OnCollisionEnter2D is used for physics-based collisions (non-triggers)
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object we collided with has the "Player" tag
        if (other.gameObject.CompareTag("Player"))
        {
            // Try to get the Health component from the player
            Health playerHealth = other.gameObject.GetComponent<Health>();

            // If the Health component exists, call the TakeDamage function
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
            }
        }
    }
}