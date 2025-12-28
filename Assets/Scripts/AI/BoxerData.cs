using UnityEngine;

[System.Serializable]
public class BoxerData
{
    [Header("Identity")]
    public string boxerName;
    public Sprite avatar;

    [Header("Resource (The Battery)")]
    public float strength;
    public float agility;
    public float stamina;
    public int totalPower; // Helper for UI

    [Header("Job Info")]
    public int level = 1;
    public int hiringCost;
    public int dailySalary;
    public float statMultiplier = 1.0f; // Multiplier for Idle Production

    public void UpdateTotal()
    {
        totalPower = Mathf.RoundToInt(strength + agility + stamina);
    }
}