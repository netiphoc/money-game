using UnityEngine;

public static class EconomyMath
{
    // Standard Cookie Clicker Price Growth (1.15x)
    public static float CalculateCost(float baseCost, int currentCount, float growthFactor = 1.15f)
    {
        return baseCost * Mathf.Pow(growthFactor, currentCount);
    }

    // Calculate how many seconds it takes to charge stats for a specific fight
    public static float CalculateTimeToCharge(float currentStats, float targetStats, float statsPerSec)
    {
        if (currentStats >= targetStats) return 0;
        return (targetStats - currentStats) / statsPerSec;
    }
}