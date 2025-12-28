using UnityEngine;

public class StatGenerator : MonoBehaviour
{
    [Header("Configuration")]
    public string equipmentID; // e.g., "Tier1_Dumbbell"
    public float baseCost = 100f;
    public float statsPerSecond = 1f; // +1 Strength/sec
    public StatType targetStat; // Strength, Agility, or Stamina

    [Header("Runtime State")]
    public int countOwned = 0; // How many of THIS item are in the room?

    public float GetBuyPrice()
    {
        return EconomyMath.CalculateCost(baseCost, countOwned);
    }

    public void BuyItem()
    {
        float price = GetBuyPrice();
        if (GameManager.Instance.TrySpendMoney(Mathf.FloorToInt(price)))
        {
            countOwned++;
            // Update UI/Visuals here
        }
    }

    // Called by the Room Manager every second
    public float ProduceStats()
    {
        return statsPerSecond * countOwned;
    }
}

public enum StatType { Strength, Agility, Stamina }