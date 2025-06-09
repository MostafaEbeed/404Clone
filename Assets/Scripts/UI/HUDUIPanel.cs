using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class HUDUIPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI boostTimerText;

    private float timerDuration;
    
    void Start()
    {
        GameManager.Instance.OnBoost += OnBoost;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnBoost -= OnBoost;
    }

    private void Update()
    {
        if (timerDuration > 0)
        {
            timerDuration -= Time.deltaTime;
            boostTimerText.text = (timerDuration).ToString("F1");
        }
        else
        {
            timerDuration = 0f;
            boostTimerText.text = (timerDuration).ToString("F1");
        }
    }

    private void OnBoost(float duration)
    {
        timerDuration = duration;
    }
}
