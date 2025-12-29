using System;
using UnityEngine;
using System.Collections.Generic;

public class GymRoom : MonoBehaviour
{
    [SerializeField] private GymRoomEquipmentDetector gymRoomEquipmentDetector;
        
    [Header("Room Info")]
    public string roomName;
    public BoxerController assignedBoxer;
    public List<TrainingEquipment> equipmentInRoom = new List<TrainingEquipment>();
    
    [Header("Capacity Limit")]
    public int maxCapacity = 10; // Default limit
    public int capacityLevel = 1;
    
    [Header("Live Production Rates (Read Only)")]
    public float totalStrRate;
    public float totalAgiRate;
    public float totalStaRate;

    private void Awake()
    {
        if (gymRoomEquipmentDetector)
        {
            gymRoomEquipmentDetector.onRoomTriggerEnter.AddListener(OnTriggerEnter);
            gymRoomEquipmentDetector.onRoomTriggerExit.AddListener(OnTriggerExit);
        }
    }

    private void OnDestroy()
    {
        if (gymRoomEquipmentDetector)
        {
            gymRoomEquipmentDetector.onRoomTriggerEnter.RemoveListener(OnTriggerEnter);
            gymRoomEquipmentDetector.onRoomTriggerExit.RemoveListener(OnTriggerExit);
        }
    }

    // --- NEW: CAPACITY CHECK ---
    public bool CanFitMore()
    {
        return equipmentInRoom.Count < maxCapacity;
    }

    // --- NEW: UPGRADE FUNCTION ---
    public void UpgradeCapacity()
    {
        capacityLevel++;
        maxCapacity += 5; // Add 5 slots per upgrade
        Debug.Log($"{roomName} capacity increased to {maxCapacity}!");
    }

    // Get the cost to upgrade (Exponential cost logic)
    public int GetUpgradeCost()
    {
        return 500 * capacityLevel; // Example: $500, $1000, $1500...
    }
    
    // Called whenever equipment is placed/removed
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
    
// --- NEW PUBLIC REMOVAL METHOD ---
    public void RemoveEquipment(TrainingEquipment equip)
    {
        if (equipmentInRoom.Contains(equip))
        {
            equipmentInRoom.Remove(equip);
            RecalculateRates();
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        TrainingEquipment equip = other.GetComponent<TrainingEquipment>();
        if (equip != null && !equipmentInRoom.Contains(equip))
        {
            equipmentInRoom.Add(equip);
            equip.currentRooms.Add(this);
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