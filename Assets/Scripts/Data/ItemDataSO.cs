using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "GymTycoon/Item Data")]
public class ItemDataSO : ScriptableObject
{
    public string itemName;
    public int cost;
    public int stackAmount;
    public GameObject itemPrefab; // The visual model inside the box
    public Sprite icon; 
    public LicenseSO licenseSo; 
    
    [Header("Consumable Buffs")]
    public bool isConsumable;
    public int consumeTimeTick;
    
    // Flat bonus to production rates
    public float strBonus; 
    public float agiBonus;
    public float staBonus;
    [SerializeField] private float hungerBonus;
    [SerializeField] private float sleepBonus; 
    
    [Header("Deployment")]
    // If this is set, this item can be taken out and placed as furniture!
    public PlaceableDataSO placementData;

    public float GetHungerBonus()
    {
        return hungerBonus / consumeTimeTick;
    }
    
    public float GetSleepBonus()
    {
        return sleepBonus / consumeTimeTick;
    }
    
}