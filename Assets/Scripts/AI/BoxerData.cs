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

    [Header("Progression")]
    public int level = 1;          // Gates which Opponents can be fought
    public float currentXP = 0;
    public float xpToNextLevel = 100;

    [Header("Job Details")]
    public int hiringCost;         // Cost to Recruit
    public int dailySalary;        // Passive maintenance cost (optional)
    public float statMultiplier = 1.0f; // Rarity Factor (e.g. 1.0 = Rookie, 1.5 = Pro)

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
}