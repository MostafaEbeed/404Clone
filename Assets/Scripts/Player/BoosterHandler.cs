using System;
using UnityEngine;

public class BoosterHandler : MonoBehaviour
{
    public static BoosterHandler Instance { get; private set; }

    
    private float boosterActiveTimer;
    private float boostBonus;
    
    public float BoosterActiveTimer => boosterActiveTimer;
    public float BoostBonus => boostBonus;
    
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

    private void Update()
    {
        if (boosterActiveTimer > 0)
        {
            boosterActiveTimer -= Time.deltaTime;
        }
        else
        {
            boosterActiveTimer = 0;
        }
    }

    public void ActivateBooster(float bonus, float boosterActiveTime)
    {
        //boosterActiveTimer += boosterActiveTime;
        boosterActiveTimer = boosterActiveTime;
        boostBonus = bonus;
        
        GameManager.Instance.OnBoost?.Invoke(boosterActiveTimer);
    }
}
