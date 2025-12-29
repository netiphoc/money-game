using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "GymTycoon/Equipment Data")]
public class PlaceableDataSO : ScriptableObject
{
    public Vector3 boxSize;
    
    [Header("Boxing Logic")]
    // When we right-click to box it up, this is the data the box will hold
    public ItemDataSO linkedItemData; 
}