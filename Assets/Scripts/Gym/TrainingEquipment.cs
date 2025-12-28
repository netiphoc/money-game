using UnityEngine;

public class TrainingEquipment : MonoBehaviour
{
    [Header("Identity")]
    public string equipmentName;
    
    [Header("Status")]
    public bool isInstalled = false; // NEW FLAG: Only true when fully placed

    [Header("Idle Production (Per Second)")]
    public float strPerSecond;
    public float agiPerSecond;
    public float staPerSecond;

    [Header("Visuals")]
    public string animationTrigger; 
    public Transform interactionPoint;

    // Safety: If this object is destroyed (Boxed up), ensure it leaves the room list
    private void OnDestroy()
    {
        // We can't rely on OnTriggerExit for destroyed objects, 
        // so we could manually notify the room here if we had a reference.
        // However, GymRoom.RecalculateRates() handles nulls usually.
        // For a robust system, the PlacementManager handles the removal logic.
    }
}