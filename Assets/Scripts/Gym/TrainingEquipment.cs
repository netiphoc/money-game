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
}