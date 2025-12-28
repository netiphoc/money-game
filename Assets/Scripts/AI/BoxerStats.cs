using UnityEngine;
using System;

[System.Serializable]
public class BoxerStats
{
    // These are "Currency", not permanent attributes
    public float currentStr;
    public float currentAgi;
    public float currentSta;

    [Header("XP System")]
    public int level = 1;
    public float currentXP;
    public float xpToNextLevel = 100f;

    // Called every second by the Room
    public void ChargeStats(float strGain, float agiGain, float staGain)
    {
        currentStr += strGain;
        currentAgi += agiGain;
        currentSta += staGain;
    }

    // The Fight Logic
    public bool TryEnterFight(float strCost, float agiCost, float staCost, int xpReward)
    {
        // 1. Check if we have enough "Fuel"
        if (currentStr >= strCost && currentAgi >= agiCost && currentSta >= staCost)
        {
            // 2. Consume the Stats (Drain)
            currentStr -= strCost;
            currentAgi -= agiCost;
            currentSta -= staCost;

            // 3. Give XP
            AddXP(xpReward);
            return true; // Win
        }
        return false; // Cannot fight yet
    }

    private void AddXP(float amount)
    {
        currentXP += amount;
        if (currentXP >= xpToNextLevel)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        level++;
        currentXP -= xpToNextLevel;
        xpToNextLevel *= 1.5f; // Harder to level up next time
        
        // Notify Unlock Manager to check for new Rooms
        Debug.Log($"Boxer Leveled Up to {level}!");
    }
}