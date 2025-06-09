using UnityEngine;

public interface IPlayerStateListener 
{
    public void OnPlayerStateChange(GameManager.PlayerState playerState);
}
