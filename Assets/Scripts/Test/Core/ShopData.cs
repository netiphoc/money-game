using UnityEngine;

[CreateAssetMenu(fileName = "NewShop", menuName = "Mall/Shop")]
public class ShopData : ScriptableObject
{
    public string shopName;
    public double baseCost; // Use double for large numbers
    public float costMultiplier = 1.15f; 
    public float roiTime = 15f; 
    public Sprite icon;
    public LicenseData[] availableLicenses;

    public double GetTotalIncomePerSecond()
    {
        return baseCost / roiTime;
    }
}