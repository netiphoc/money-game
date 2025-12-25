using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "GymTycoon/Item Data")]
public class ItemDataSO : ScriptableObject
{
    public string itemName;
    public GameObject itemPrefab; // The visual model inside the box
    public Sprite icon; 
    
    [Header("Deployment")]
    // If this is set, this item can be taken out and placed as furniture!
    public PlaceableDataSO placementData; 
}