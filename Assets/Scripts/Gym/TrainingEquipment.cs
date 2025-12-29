using UnityEngine;

public class TrainingEquipment : MonoBehaviour
{
    [Header("Idle Production (Per Second)")]
    public float strPerSecond;
    public float agiPerSecond;
    public float staPerSecond;

    [Header("Visuals")]
    public string animationTrigger; 
    public Transform interactionPoint;
    
    // NEW: Track which room owns this equipment
    public GymRoom currentRoom;
    
    // --- CLEANUP LOGIC ---
    private void OnDestroy()
    {
        // If this object is destroyed (Boxed Up), tell the room immediately!
        if (currentRoom != null)
        {
            currentRoom.RemoveEquipment(this);
        }
    }
}