using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New License", menuName = "GymTycoon/License Data")]
public class LicenseSO : ScriptableObject
{
    public Sprite icon;
    public string licenseName; // e.g., "Amateur League"
    [TextArea] public string description;
    public int cost;
    public int requiredBoxerLevel; // Can't buy license until Boxer is Lvl 5
    
    [Header("Unlocks")]
    // The equipment this license allows you to buy
    public List<PlaceableDataSO> unlockedEquipment; 
}