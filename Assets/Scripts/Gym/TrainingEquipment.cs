using System;
using System.Collections.Generic;
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
    public List<GymRoom> currentRooms = new List<GymRoom>();
    
    // --- CLEANUP LOGIC ---
    private void OnDestroy()
    {
        foreach (var t in currentRooms)
        {
            t.RemoveEquipment(this);
        }
    }
}