using UnityEngine;

[CreateAssetMenu(fileName = "NewLicense", menuName = "Mall/License")]
public class LicenseData : ScriptableObject
{
    [Header("Display Info")]
    public string licenseName;
    [TextArea] public string description;
    public Sprite icon;

    [Header("Economy Settings")]
    // Use double to prevent money overflow in late-game
    public double unlockCost; 
    
    // How much this boosts the specific shop's income (e.g., 1.25 for +25%)
    public float incomeMultiplier = 1.25f;

    [Header("Progression")]
    // Optional: Make this license require a previous one (Tier 1 -> Tier 2)
    public LicenseData requiredLicense;
}