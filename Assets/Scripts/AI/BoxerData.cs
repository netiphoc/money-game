using System;
using UnityEngine;

[System.Serializable]
public class BoxerData
{
    public void InitStats()
    {
        hunger = 100;
        sleep = 100;
        level = 1;
        xpToNextLevel = 100;
        hungerDecayRate = 0.5f;
        sleepDecayRate = 0.2f;
    }
    
    [Header("Identity")]
    public string boxerName;
    public Sprite avatar; 

    [Header("Resource Battery (The Fuel)")]
    public float strength;
    public float agility;
    public float stamina;
    public int totalPower; 
    
    [Header("Unrealized Resource Battery (The Fuel)")]
    public float unrealizedStrength;
    public float unrealizedAgility;
    public float unrealizedStamina;
    public float unrealizedSleep;
    public float unrealizedHunger;

    [Header("Survival Stats (0-100)")]
    public float hunger = 100f;
    public float sleep = 100f;
    
    // Decay Rates (How fast they get hungry)
    public float hungerDecayRate = 0.5f; // Lose 1 hunger every 2 seconds
    public float sleepDecayRate = 0.2f;  // Lose 1 sleep every 5 seconds
    
    [Header("Progression")]
    public int level = 1;          // Gates which Opponents can be fought
    public float currentXP = 0;
    public float xpToNextLevel = 100;

    public event Action<float> OnExpChanged;
    public event Action<int> OnLevelChanged;

    public void UpdateTotal()
    {
        totalPower = Mathf.RoundToInt(strength + agility + stamina);
    }

    public void AddXp(float amount)
    {
        currentXP += amount;
        OnExpChanged?.Invoke(currentXP);
        
        // Level Up Loop (in case we get massive XP at once)
        while (currentXP >= xpToNextLevel)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        currentXP -= xpToNextLevel;
        level++;
        xpToNextLevel *= 1.5f; // 50% harder each level
        OnLevelChanged?.Invoke(level);
        Debug.Log($"{boxerName} leveled up to Lvl {level}!");
    }

    public void ApplyUnrealizeStats()
    {
        strength += unrealizedStrength;
        agility += unrealizedAgility;
        stamina += unrealizedStamina;
        
        sleep += unrealizedSleep;
        hunger += unrealizedHunger;
        
        unrealizedStrength = 0;
        unrealizedAgility = 0;
        unrealizedStamina = 0;
        
        unrealizedSleep = 0;
        unrealizedHunger = 0;
        
        UpdateTotal();
    }

    #region Food
    public void TickSurvival()
    {
        // 1. Natural Decay
        hunger = Mathf.Max(0, hunger - hungerDecayRate);
        sleep = Mathf.Max(0, sleep - sleepDecayRate);
    }

    // Call this in IdleProductionManager to get the penalty multiplier
    public float GetEfficiencyMultiplier()
    {
        float mult = 1.0f;

        // Hunger Penalty
        if (hunger <= 0) mult *= 0.1f;       // Starving: 10% speed
        else if (hunger < 20) mult *= 0.5f;  // Hungry: 50% speed

        // Sleep Penalty
        if (sleep <= 0) mult *= 0.0f;        // Asleep: 0% speed (Stops working)
        else if (sleep < 15) mult *= 0.25f;  // Exhausted: 25% speed

        return mult;
    }
    
    #endregion
}