using System;
using UnityEngine;
using System.Collections.Generic;

public class GymRoom : MonoBehaviour
{
    [SerializeField] private GymRoomEquipmentDetector gymRoomEquipmentDetector;
        
    [Header("Room Info")]
    public BoxerController assignedBoxer;
    public List<TrainingEquipment> equipmentInRoom = new List<TrainingEquipment>();

    [Header("Capacity Limit")]
    public int maxCapacity = 10; // Default limit
    public int capacityLevel = 1;
    
    [Header("Live Production Rates (Read Only)")]
    public float totalStrRate;
    public float totalAgiRate;
    public float totalStaRate;
    public event Action<GymRoom> OnRoomUnlocked;
    public bool IsUnlocked { get; private set; }

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
            
            // Check consume able
            if (equip.TryGetConsumableShelf(out StorageShelf[] shelves))
            {
                foreach (var shelf in shelves)
                {
                    ItemDataSO itemDataSo = shelf.GetItem();

                    for (int i = 0; i < shelf.GetItemCount(); i++)
                    {
                        // Apply stat
                        totalStrRate += itemDataSo.strBonus;
                        totalAgiRate += itemDataSo.agiBonus;
                        totalStaRate += itemDataSo.staBonus;
                    }
                }
            }
        }

        // Optional: Apply License Multipliers here
        // totalStrRate *= LicenseManager.Instance.GetMultiplier();
    }
    
// --- NEW PUBLIC REMOVAL METHOD ---
    public void RemoveEquipment(TrainingEquipment equip)
    {
        if (equipmentInRoom.Contains(equip))
        {
            equip.OnStorageShelfUpdated -= OnStorageShelfUpdated;
            equipmentInRoom.Remove(equip);
            RecalculateRates();
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        TrainingEquipment equip = other.GetComponent<TrainingEquipment>();
        if (equip != null && !equipmentInRoom.Contains(equip))
        {
            equip.OnStorageShelfUpdated += OnStorageShelfUpdated;
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
            equip.OnStorageShelfUpdated -= OnStorageShelfUpdated;
            equipmentInRoom.Remove(equip);
            RecalculateRates();
        }
    }

    private void OnStorageShelfUpdated(StorageShelf storageShelf)
    {
        Debug.Log("OnStorageShelfUpdated");
        RecalculateRates();
    }

    public void UnlockRoom(GymRoom gymRoom)
    {
        IsUnlocked = true;
        OnRoomUnlocked?.Invoke(gymRoom);
    }
}