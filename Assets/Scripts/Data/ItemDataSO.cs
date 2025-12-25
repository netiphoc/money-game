using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "GymTycoon/Item Data")]
public class ItemDataSO : ScriptableObject
{
    public string itemName;
    public GameObject itemPrefab; // The visual model of the single item (e.g., one bottle)
    public Sprite icon; 
}