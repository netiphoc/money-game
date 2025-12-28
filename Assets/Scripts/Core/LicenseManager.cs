using UnityEngine;
using System.Collections.Generic;

public class LicenseManager : MonoBehaviour
{
    public static LicenseManager Instance;

    [Header("Database")]
    public List<LicenseSO> allLicenses; // Drag all 7 tiers here

    [Header("Save Data")]
    public List<LicenseSO> ownedLicenses = new List<LicenseSO>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        // Always give Tier 1 for free
        if (allLicenses.Count > 0 && !ownedLicenses.Contains(allLicenses[0]))
        {
            ownedLicenses.Add(allLicenses[0]);
        }
    }

    public bool IsEquipmentUnlocked(PlaceableDataSO equipment)
    {
        // Check if any owned license contains this equipment
        foreach (var license in ownedLicenses)
        {
            if (license.unlockedEquipment.Contains(equipment)) return true;
        }
        return false;
    }

    public bool IsLicenseUnlocked(LicenseSO license)
    {
        return ownedLicenses.Contains(license);
    }

    public bool CanBuyLicense(LicenseSO license)
    {
        // 1. Check if already owned
        if (ownedLicenses.Contains(license)) return false;

        // 2. Check Boxer Level (Assuming you have a reference to the main boxer)
        // int currentLevel = BoxerManager.Instance.mainBoxer.stats.level;
        // if (currentLevel < license.requiredBoxerLevel) return false;

        // 3. Check Money
        return GameManager.Instance.currentMoney >= license.cost;
    }

    public void BuyLicense(LicenseSO license)
    {
        if (CanBuyLicense(license))
        {
            if (GameManager.Instance.TrySpendMoney(license.cost))
            {
                ownedLicenses.Add(license);
                Debug.Log($"Unlocked: {license.licenseName}");
                // Update Shop UI event
            }
        }
    }
}