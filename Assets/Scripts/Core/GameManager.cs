using UnityEngine;
using System;
using Core;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("System")]
    [field: SerializeField] public GameTimeManager GameTimeManager { get; private set; }
    [field: SerializeField] public GameController GameController { get; private set; }

    [Header("Economy")]
    public int currentMoney = 1000; // Starting cash
    public int currentExp = 0;
    
    [Header("Global Progression (Gym Level)")]
    public int playerLevel = 1;      // Used for unlocking Rooms
    public float playerXP = 0;
    public float xpToNextLevel = 1000;
    
    // Events allow the UI to update automatically when money changes
    public event Action<int> OnMoneyChanged;
    public event Action<int> OnLevelChanged;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    #region ECONOMY

    public bool TrySpendMoney(int amount)
    {
        if (currentMoney >= amount)
        {
            currentMoney -= amount;
            OnMoneyChanged?.Invoke(currentMoney);
            return true;
        }
        return false;
    }

    public void AddMoney(int amount)
    {
        currentMoney += amount;
        OnMoneyChanged?.Invoke(currentMoney);
    }

    

    #endregion
    #region LEVEL

    public void AddPlayerXP(float amount)
    {
        playerXP += amount;
        if (playerXP >= xpToNextLevel)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        playerLevel++;
        playerXP -= xpToNextLevel;
        xpToNextLevel *= 1.2f; // Harder to level up next time
        
        Debug.Log($"GYM LEVEL UP! Now Level {playerLevel}");
        OnLevelChanged?.Invoke(playerLevel);
        
        // Optional: Play Level Up Sound / Confetti
    }
    

    #endregion
}