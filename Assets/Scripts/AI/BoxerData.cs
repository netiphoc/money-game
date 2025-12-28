using UnityEngine;

[System.Serializable]
public class BoxerData
{
    public string boxerName;
    
    [Header("Core Stats")]
    public float strength;
    public float agility;
    public float stamina;
    public int level;
    
    [Header("Progression")]
    public int totalTrainingPoints; // Sum of stats (used for matchmaking)
    
    [Header("Status")]
    public float energy = 100f; // 0 = Too tired to train
    public float hunger = 0f;   // 100 = Too hungry to train

    public void AddStats(float str, float agi, float sta)
    {
        strength += str;
        agility += agi;
        stamina += sta;
        
        UpdateTotal();
    }

    public void UpdateTotal()
    {
        totalTrainingPoints = Mathf.RoundToInt(strength + agility + stamina);
    }
}