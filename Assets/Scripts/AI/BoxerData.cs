using System;
using UnityEngine;

[System.Serializable]
public class BoxerData
{
    [Header("Identity")]
    public string boxerName;
    public Sprite avatar; // Icon for the Tablet/UI

    [Header("Resource Battery (The Fuel)")]
    // These float up continuously based on room production
    public float strength;
    public float agility;
    public float stamina;
    
    // The integer sum used to determine if you can win a fight
    public int totalPower; 
    
    [Header("Unrealized Resource Battery (The Fuel)")]
    // These float up continuously based on room production
    public float unrealizedStrength;
    public float unrealizedAgility;
    public float unrealizedStamina;
    public float unrealizedSleep;
    public float unrealizedHunger;

    [Header("Progression")]
    public int level = 1;          // Gates which Opponents can be fought
    public float currentXP = 0;
    public float xpToNextLevel = 100;

    [Header("Job Details")]
    public int hiringCost;         // Cost to Recruit
    public int dailySalary;        // Passive maintenance cost (optional)
    public float statMultiplier = 1.0f; // Rarity Factor (e.g. 1.0 = Rookie, 1.5 = Pro)
    
    [Header("Survival Stats (0-100)")]
    public float hunger = 100f;
    public float sleep = 100f;
    
    // Decay Rates (How fast they get hungry)
    public float hungerDecayRate = 0.5f; // Lose 1 hunger every 2 seconds
    public float sleepDecayRate = 0.2f;  // Lose 1 sleep every 5 seconds
    
    public event Action<float> OnExpChanged;
    public event Action<int> OnLevelChanged;

    // =========================================================
    // HELPER METHODS
    // =========================================================

    // Called by IdleProductionManager every tick
    public void UpdateTotal()
    {
        totalPower = Mathf.RoundToInt(strength + agility + stamina);
    }

    // Called by FightManager when winning a fight
    public void AddXP(float amount)
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

    // Called by TabletUI to check if button should be clickable
    public bool CanAffordFightCost(int strCost, int agiCost, int staCost)
    {
        return strength >= strCost && agility >= agiCost && stamina >= staCost;
    }

    // Called by FightManager to "Pay" for the fight
    public void ConsumeStats(int strCost, int agiCost, int staCost)
    {
        strength = Mathf.Max(0, strength - strCost);
        agility = Mathf.Max(0, agility - agiCost);
        stamina = Mathf.Max(0, stamina - staCost);
        
        UpdateTotal();
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