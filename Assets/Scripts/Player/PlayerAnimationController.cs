using UnityEngine;

public class PlayerAnimationController : MonoBehaviour, IGameStateListener
{
    [SerializeField] private Animator animator;
    
    void Start()
    {
        if(animator == null)
            animator = GetComponent<Animator>();
    }

    public void Jump()
    {
        animator.ResetTrigger("Run");
        animator.SetTrigger("Jump"); 
    }

    public void Run()
    {
        animator.ResetTrigger("Jump");
        animator.SetTrigger("Run");
    }

    public void OnGameStateChange(GameManager.GameState gameState)
    {
        if (gameState == GameManager.GameState.Gameplay)
        {
            Run();
        }
        else
        {
            Jump();
        }
    }
}
