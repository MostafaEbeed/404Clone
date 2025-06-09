using UnityEngine;

public class EnvColorHandler : MonoBehaviour, IPlayerStateListener
{
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        if(spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        
        HandleColorOnStart();
    }


    public void OnPlayerStateChange(GameManager.PlayerState playerState)
    {
        if (playerState == GameManager.PlayerState.Normal)
        {
            Color newColor = Color.black;
            spriteRenderer.color = newColor;
        }
        else //boosted
        {
            Color newColor = Color.red;
            spriteRenderer.color = newColor;
        }
    }

    private void HandleColorOnStart()
    {
        if (GameManager.Instance.CurrentPlayerState == GameManager.PlayerState.Normal)
        {
            Color newColor = Color.black;
            spriteRenderer.color = newColor;
        }
        else
        {
            Color newColor = Color.red;
            spriteRenderer.color = newColor;
        }
    }
}
