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
    
    [Header("Deployment")]
    // If this is set, this item can be taken out and placed as furniture!
    public PlaceableDataSO placementData; 
}