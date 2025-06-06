using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{
    // Singleton pattern: A single, globally accessible instance of the GameManager
    public static GameManager Instance { get; private set; }

    // Define the possible states of our game
    public enum GameState
    {
        StartMenu,
        Countdown,
        Gameplay,
        GameOver
    }

    // The current state of the game
    public GameState CurrentState;

    // Event to notify other parts of the game when the state changes
    // UI and Sound managers will subscribe to this event.
    public static event Action<GameState> OnStateChanged;

    [Header("Game Speed Settings")]
    [SerializeField] private float initialMoveSpeed = 5f;
    [SerializeField] private float maxMoveSpeed = 20f;
    [SerializeField] private float speedIncreaseRate = 0.1f; // Speed added per second

    public float CurrentMoveSpeed { get; private set; }

    private void Awake()
    {
        // Implement the Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject); // Optional: if you have multiple scenes
        }
    }

    private void Start()
    {
        // The game starts in the main menu state
        SetState(GameState.StartMenu);
    }

    private void Update()
    {
        // Increase speed over time only during gameplay
        if (CurrentState == GameState.Gameplay)
        {
            if (CurrentMoveSpeed < maxMoveSpeed)
            {
                CurrentMoveSpeed += speedIncreaseRate * Time.deltaTime;
            }
            else
            {
                CurrentMoveSpeed = maxMoveSpeed;
            }
        }
        
        // --- FOR TESTING ---
        // Simple way to start the game from the editor without UI buttons
        if (CurrentState == GameState.StartMenu && Input.GetKeyDown(KeyCode.Space))
        {
            StartGame();
        }
    }

    public void SetState(GameState newState)
    {
        //if (newState == CurrentState) return;

        CurrentState = newState;
        
        // Call the event to notify listeners (like UI, Sound, Player)
        OnStateChanged?.Invoke(newState);

        BroadCastToAllGameStateListeners(newState);
        
        // Handle logic for entering each state
        switch (newState)
        {
            case GameState.StartMenu:
                HandleStartMenu();
                break;
            case GameState.Countdown:
                StartCoroutine(CountdownRoutine());
                break;
            case GameState.Gameplay:
                HandleGameplay();
                break;
            case GameState.GameOver:
                HandleGameOver();
                break;
        }
    }
    
    private void BroadCastToAllGameStateListeners(GameState gameState)
    {
        IEnumerable<IGameStateListener> gameStateListeners =
            FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IGameStateListener>();

        foreach (IGameStateListener gameStateListener in gameStateListeners)
        {
            gameStateListener.OnGameStateChange(gameState);
        }
    }

    
    // --- Public methods to be called by other scripts (e.g., UI buttons) ---
    
    public void StartGame()
    {
        // This can be called by a "Start" button in your UI
        SetState(GameState.Countdown);
    }
    
    public void EndGame()
    {
        // This will be called by the player script when it collides with an obstacle
        SetState(GameState.GameOver);
    }

    // --- State-specific logic ---

    private void HandleStartMenu()
    {
        Debug.Log("State: Start Menu. Press Space to begin.");
        CurrentMoveSpeed = 0f;
        // Here you would typically show the main menu UI
    }

    private IEnumerator CountdownRoutine()
    {
        Debug.Log("State: Countdown");
        // Here you would show a "3, 2, 1" countdown on the screen
        yield return new WaitForSeconds(3f); // Wait for 3 seconds
        SetState(GameState.Gameplay);
    }
    
    private void HandleGameplay()
    {
        Debug.Log("State: Gameplay. The game is running!");
        CurrentMoveSpeed = initialMoveSpeed;
        // Here you would hide menu UI and enable game UI (like score)
    }

    private void HandleGameOver()
    {
        Debug.Log("State: Game Over");
        CurrentMoveSpeed = 0f; // Stop the world from moving
        // Here you would show the "Game Over" screen with score and a restart button
    }
}