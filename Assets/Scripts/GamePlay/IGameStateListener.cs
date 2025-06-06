using UnityEngine;

public interface IGameStateListener 
{
    public void OnGameStateChange(GameManager.GameState gameState);
}
