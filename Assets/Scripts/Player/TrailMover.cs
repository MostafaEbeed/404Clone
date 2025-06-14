using UnityEngine;

public class TrailMover : MonoBehaviour
{
    void Update()
    {
        // Only move if the game is in the Gameplay state
        if (GameManager.Instance.CurrentState != GameManager.GameState.Gameplay) return;
        
        // Move the object to the left based on the current speed from the GameManager
        float speed = GameManager.Instance.EffectiveMoveSpeed;
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }
}
