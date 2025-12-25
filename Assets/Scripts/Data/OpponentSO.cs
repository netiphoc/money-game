using UnityEngine;

[CreateAssetMenu(fileName = "New Opponent", menuName = "GymTycoon/Opponent Data")]
public class OpponentSO : ScriptableObject
{
    [Header("Identity")]
    public string opponentName;
    public Sprite avatar; // Face icon for UI
    public string description;

    [Header("Stats")]
    public int strength;
    public int agility;
    public int stamina;

    [Header("Rewards")]
    public int moneyReward;
    public int xpReward;
    public int trophyReward = 1; // Only given on Win
    
    // Helper to get total power
    public int TotalPower => strength + agility + stamina;
}