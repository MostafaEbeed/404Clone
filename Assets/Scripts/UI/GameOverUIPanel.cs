using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUIPanel : MonoBehaviour
{
    [SerializeField] private Timer timer;
    [SerializeField] private TextMeshProUGUI timerText;

    private void Start()
    {
        UpdateTimerText();
        
        Invoke("LoadLevel", 5f);
    }

    private void UpdateTimerText()
    {
        timerText.text = timer.CurentTime.ToString("00");
    }

    private void LoadLevel()
    {
        SceneManager.LoadScene(0);
    }
}
