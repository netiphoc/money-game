using UnityEngine;

[CreateAssetMenu(fileName = "New FightDataSO", menuName = "GymTycoon/Fight Data")]
public class FightDataSO : ScriptableObject
{
    public string fightName;
    public OpponentSO[] opponents;
}