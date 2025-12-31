using UnityEngine;
using System;
using Core;
using SaveLoadSystem;
using StarterAssets;

public enum SpendType
{
    SUPPLY, UPGRADE
}

public class GameManager : MonoBehaviour, ISaveLoadSystem
{
    
    public static GameManager Instance;

    [Header("System")]
    [field: SerializeField] public FirstPersonController FirstPersonController{ get; private set; }
    [field: SerializeField] public PlayerInteraction PlayerInteraction { get; private set; }
    [field: SerializeField] public GameTimeManager GameTimeManager { get; private set; }
    [field: SerializeField] public GameController GameController { get; private set; }
    [field: SerializeField] public GymRoom[] GymRooms { get; private set; }

    [Header("Time")]
    public int totalDays = 1;
    
    [Header("Economy")]
    public int currentMoney = 1000; // Starting cash
    
    [Header("Global Progression (Gym Level)")]
    public int playerLevel = 1;      // Used for unlocking Rooms
    public float playerXP = 0;
    public float xpToNextLevel = 50f;
    public float levelDifficultyMultiplier = 1.15f;
    
    [Header("Summary")]
    public float totalIncome;
    public float totalSupplyCosts;
    public float totalUpgradeCosts;
    
    // Events allow the UI to update automatically when money changes
    public event Action<int> OnMoneyChanged;
    public event Action<float, float> OnExpChanged;
    public event Action<int> OnLevelChanged;
    public event Action<int> OnDayChanged;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    #region ECONOMY

    public bool TrySpendMoney(int amount, SpendType spendType)
    {
        if (currentMoney >= amount)
        {
            currentMoney -= amount;
            
            switch (spendType)
            {
                case SpendType.SUPPLY:
                    totalSupplyCosts += amount;
                    break;
                case SpendType.UPGRADE:
                    totalUpgradeCosts += amount;
                    break;
            }
            
            FloatingTextManager.Instance.ShowFixedText($"-${amount:F0}", Color.red, TextSpawnPointType.Money);
            OnMoneyChanged?.Invoke(currentMoney);
            return true;
        }
        return false;
    }

    public void AddMoney(int amount)
    {
        currentMoney += amount;
        totalIncome += amount;
        OnMoneyChanged?.Invoke(currentMoney);
    }

    public void AddDay()
    {
        totalDays++;
    }

    #endregion
    #region LEVEL

    public void AddPlayerXP(float amount)
    {
        playerXP += amount;
        
        // Check for level up loop (in case we get massive XP at once)
        while (playerXP >= xpToNextLevel)
        {
            LevelUp();
        }

        OnExpChanged?.Invoke(playerXP, xpToNextLevel);
    }

    private void LevelUp()
    {
        playerXP -= xpToNextLevel;
        playerLevel++;

        // Exponential Curve: 50 -> 57 -> 66 -> 76...
        xpToNextLevel *= levelDifficultyMultiplier; 
        
        Debug.Log($"GYM LEVEL UP! Now Level {playerLevel}");
        OnLevelChanged?.Invoke(playerLevel);
        
        // Notify UI to update the bar with new max
        OnExpChanged?.Invoke(playerXP, xpToNextLevel);
    }

    public void SetAllowPlayerInteraction(bool canInteract)
    {
        FirstPersonController.SetAllowedMovement(canInteract);
        PlayerInteraction.enabled = canInteract;
    }

    #endregion

    public void SaveGame()
    {
        PlayerPrefs.SetInt($"{name}_totalDays", totalDays);
        PlayerPrefs.SetInt($"{name}_currentMoney", currentMoney);
        PlayerPrefs.SetInt($"{name}_playerLevel", playerLevel);
        PlayerPrefs.SetFloat($"{name}_playerXP", playerXP);
        PlayerPrefs.SetFloat($"{name}_xpToNextLevel", xpToNextLevel);
    }

    public void LoadGame()
    {
        totalDays = PlayerPrefs.GetInt($"{name}_totalDays", totalDays);
        currentMoney = PlayerPrefs.GetInt($"{name}_currentMoney", currentMoney);
        playerLevel = PlayerPrefs.GetInt($"{name}_playerLevel", playerLevel);
        playerXP = PlayerPrefs.GetFloat($"{name}_playerXP", playerXP);
        xpToNextLevel = PlayerPrefs.GetFloat($"{name}_xpToNextLevel", xpToNextLevel);
        OnMoneyChanged?.Invoke(currentMoney);
        OnExpChanged?.Invoke(playerXP, xpToNextLevel);
        OnLevelChanged?.Invoke(playerLevel);
        OnDayChanged?.Invoke(totalDays);
    }
}