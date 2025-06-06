using UnityEngine;

public class WorldMover : MonoBehaviour
{
    void Update()
    {
        // Only move if the game is in the Gameplay state
        if (GameManager.Instance.CurrentState != GameManager.GameState.Gameplay) return;
        
        // Move the object to the left based on the current speed from the GameManager
        transform.Translate(Vector3.left * GameManager.Instance.CurrentMoveSpeed * Time.deltaTime);
    }
}