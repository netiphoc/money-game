using UnityEngine;

[CreateAssetMenu(fileName = "New Opponent", menuName = "GymTycoon/Opponent Data")]
public class OpponentSO : ScriptableObject
{
    public string opponentName;
    public Sprite avatar;
    public int strength;
    public int agility;
    public int stamina;

    [Header("Requirements")]
    public int requiredBoxerLevel; // The BOXER must be this level to fight

    [Header("Rewards")]
    public int moneyReward;
    public int boxerXPReward; // XP for the Boxer
    public int playerXPReward; // XP for the Gym (You)
    
    public int TotalPower => strength + agility + stamina;
}