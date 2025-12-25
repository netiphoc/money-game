using UnityEngine;

public class TrainingEquipment : MonoBehaviour
{
    [Header("Stats")]
    public string equipmentName;
    public float trainingDuration = 5.0f; // Seconds per cycle
    
    [Header("Gains")]
    public float strGain;
    public float agiGain;
    public float staGain;

    [Header("Animation")]
    public string animationTrigger; // e.g., "Run", "Punch"
    public Transform interactionPoint; // Where the boxer stands (Green Arrow logic)

    private void OnDrawGizmos()
    {
        if (interactionPoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(interactionPoint.position, 0.3f);
            Gizmos.DrawLine(interactionPoint.position, interactionPoint.position + interactionPoint.forward * 0.5f);
        }
    }
}