using UnityEngine;

public class Timer : MonoBehaviour, IGameStateListener
{
    private float timer;

    private bool started = false;

    public float CurentTime => timer;
    
    void Update()
    {
        if(started)
            timer += Time.deltaTime;
    }


    public void OnGameStateChange(GameManager.GameState gameState)
    {
        started = false;
        if (gameState == GameManager.GameState.Gameplay)
        {
            started = true;
        }
    }
}
