using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour, IGameStateListener
{
    public static PlayerController Instance { get; private set; }
    
    [Header("Player Settings")] 
    [SerializeField]private GameObject player;
    
    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 15f;
    [SerializeField] private float gravityMultiplier = 3f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Swipe Input")]
    [SerializeField] private float minSwipeDistanceY = 50f; // Minimum pixels to be considered a swipe

    private Rigidbody2D rb;
    private bool isGrounded;
    private Vector2 touchStartPos;
    
    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
        
        rb = GetComponent<Rigidbody2D>();

    }
    
    void OnEnable()
    {
        // Subscribe to the state change event
        GameManager.OnStateChanged += HandleGameStateChange;
    }

    void OnDisable()
    {
        // Always unsubscribe to prevent memory leaks
        GameManager.OnStateChanged -= HandleGameStateChange;
    }

    void Update()
    {
        // Player can only act during the Gameplay state
        if (GameManager.Instance.CurrentState != GameManager.GameState.Gameplay) return;
        
        // Update the ground check status
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Handle Input
        HandleTouchInput();
        // --- FOR TESTING ---
        HandleKeyboardInput();

        HandleGravity();
    }
    
    private void HandleGameStateChange(GameManager.GameState newState)
    {
        // Control the player's physics based on game state
        if (newState == GameManager.GameState.Gameplay)
        {
            rb.bodyType = RigidbodyType2D.Dynamic; // Let physics take over
        }
        else
        {
            rb.bodyType = RigidbodyType2D.Kinematic; // Stop all physics movement
            rb.linearVelocity = Vector2.zero;
        }
    }
    
    private void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                touchStartPos = touch.position;
            }
            
            if (touch.phase == TouchPhase.Ended)
            {
                float swipeDeltaY = touch.position.y - touchStartPos.y;
                
                // Check if it was a clear upward swipe
                if (swipeDeltaY > minSwipeDistanceY)
                {
                    Jump();
                }
            }
        }
    }

    private void HandleKeyboardInput()
    {
        // For easy testing in the editor
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            Jump();
        }
    }

    private void Jump()
    {
        if (isGrounded)
        {
            // Reset vertical velocity for a consistent jump height
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
            // Apply an instant upward force
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    private void HandleGravity()
    {
        if (isGrounded)
        {
            Physics2D.gravity = new Vector2(0f, -9.81f);
        }
        else
        {
            Physics2D.gravity = new Vector2(0f, -9.81f * gravityMultiplier);
        }
    }
    
    // This is how you'll trigger the game over state later
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Make sure your obstacles have a tag like "Obstacle"
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            GameManager.Instance.EndGame();
        }
    }

    // Gizmo for visualizing the ground check circle in the editor
    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }

    public void OnGameStateChange(GameManager.GameState gameState)
    {
        Debug.Log("Here");

        if (gameState != GameManager.GameState.Gameplay)
        {
            player.SetActive(false);
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.simulated = false;
        }
        else
        {
            player.SetActive(true);
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.simulated = true;
        }
    }
}