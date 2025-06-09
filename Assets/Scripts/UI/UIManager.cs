using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour, IGameStateListener
{
    [Header("HUD UI Elements")]
    [SerializeField] private Timer timer;
    [SerializeField] private TextMeshProUGUI timerText;
    private bool shouldUpdateTimerUI = false;
    
    [Header("UI Panels")]
    [SerializeField] private GameObject startMenuPanel;
    [SerializeField] private GameObject gamePlayPanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject countDownPanel;
    
    void Start()
    {
        
    }

    void Update()
    {
        if(shouldUpdateTimerUI)
            UpdateTimerUI();
    }

    private void UpdateTimerUI()
    {
        timerText.text = timer.CurentTime.ToString("00");
    }

    public void OnGameStateChange(GameManager.GameState gameState)
    {
        shouldUpdateTimerUI = false;
        
        if (gameState == GameManager.GameState.Gameplay)
        {
            ShowOnlyOnePanel(gamePlayPanel);
            shouldUpdateTimerUI = true;
        }
        else if (gameState == GameManager.GameState.GameOver)
        {
            ShowOnlyOnePanel(gameOverPanel);
        }
        else if (gameState == GameManager.GameState.Countdown)
        {
            ShowOnlyOnePanel(countDownPanel);
            ShowUIPanel(gamePlayPanel);
        }
        else
        {
            ShowOnlyOnePanel(startMenuPanel);
        }
    }

    private void ShowUIPanel(GameObject panel)
    {
        panel.SetActive(true);
    }

    private void HideUIPanel(GameObject panel)
    {
        panel.SetActive(false);
    }

    private void ShowOnlyOnePanel(GameObject panel)
    {
        HideUIPanel(startMenuPanel);
        HideUIPanel(gamePlayPanel);
        HideUIPanel(gameOverPanel);
        
        panel.SetActive(true);
    }
}
