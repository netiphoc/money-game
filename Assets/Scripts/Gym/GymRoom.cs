using UnityEngine;
using System.Collections.Generic;

public class GymRoom : MonoBehaviour
{
    [Header("Room Info")]
    public string roomName;
    public BoxerController assignedBoxer;
    public List<TrainingEquipment> equipmentInRoom = new List<TrainingEquipment>();

    [Header("Live Production Rates (Read Only)")]
    public float totalStrRate;
    public float totalAgiRate;
    public float totalStaRate;

    // Called whenever equipment is placed/removed
    [ContextMenu("RecalculateRates")]
    public void RecalculateRates()
    {
        totalStrRate = 0;
        totalAgiRate = 0;
        totalStaRate = 0;

        foreach (var equip in equipmentInRoom)
        {
            totalStrRate += equip.strPerSecond;
            totalAgiRate += equip.agiPerSecond;
            totalStaRate += equip.staPerSecond;
        }

        // Optional: Apply License Multipliers here
        // totalStrRate *= LicenseManager.Instance.GetMultiplier();
    }

    private void OnTriggerEnter(Collider other)
    {
        TrainingEquipment equip = other.GetComponent<TrainingEquipment>();
        if (equip != null && !equipmentInRoom.Contains(equip))
        {
            equipmentInRoom.Add(equip);
            RecalculateRates(); // Update math immediately
        }
        
        // ... (Boxer detection remains same)
    }

    private void OnTriggerExit(Collider other)
    {
        TrainingEquipment equip = other.GetComponent<TrainingEquipment>();
        if (equip != null && equipmentInRoom.Contains(equip))
        {
            equipmentInRoom.Remove(equip);
            RecalculateRates();
        }
    }
}