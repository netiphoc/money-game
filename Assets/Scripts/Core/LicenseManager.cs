using System;
using UnityEngine;
using System.Collections.Generic;
using SaveLoadSystem;

public class LicenseManager : MonoBehaviour, ISaveLoadSystem
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
            if (GameManager.Instance.TrySpendMoney(license.cost, SpendType.UPGRADE))
            {
                ownedLicenses.Add(license);
                Debug.Log($"Unlocked: {license.licenseName}");
                // Update Shop UI event
            }
        }
    }
    
    [Serializable]
    public class LicenseSaveData
    {
        public string[] licenseIds;
    }

    public void SaveGame()
    {
        List<string> licenses = new List<string>();
        foreach (var license in ownedLicenses)
        {
            licenses.Add(license.licenseName);
        }

        string jsonLicense = JsonUtility.ToJson(new LicenseSaveData
        {
            licenseIds = licenses.ToArray(),
        });
        
        PlayerPrefs.SetString($"{name}_license", jsonLicense);
    }

    public void LoadGame()
    {
        if (PlayerPrefs.HasKey($"{name}_license"))
        {
            int skip = ownedLicenses.Count;
            LicenseSaveData data = JsonUtility.FromJson<LicenseSaveData>(PlayerPrefs.GetString($"{name}_license"));
            foreach (var licenseId in data.licenseIds)
            {
                if(skip-- > 0) continue;
                foreach (var license in allLicenses)
                {
                    if(!license.licenseName.Equals(licenseId)) continue;
                    ownedLicenses.Add(license);
                }
            }
        }
    }
}