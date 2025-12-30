using System;
using UnityEngine;
using System.Collections.Generic;
using Data;

public class GymRoom : MonoBehaviour
{
    [SerializeField] private GymRoomEquipmentDetector gymRoomEquipmentDetector;

    [field: SerializeField] public Transform SpawnPoint { get; private set; } 
    [field: SerializeField] public RoomDataSO RoomDataSo { get; private set; } 

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
    public float totalSleepRate;
    public float totalHungerRate;
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
        totalSleepRate = 0;
        totalHungerRate = 0;

        List<PlaceableDataSO> addedItems = new List<PlaceableDataSO>();
        foreach (var equip in equipmentInRoom)
        {
            bool hasShelf = equip.TryGetConsumableShelf(out StorageShelf[] shelves);
            
            // Check consume able
            if (hasShelf)
            {
                foreach (var shelf in shelves)
                {
                    ItemDataSO itemDataSo = shelf.GetItem();

                    for (int i = 0; i < shelf.GetItemCount(); i++)
                    {
                    }
                    
                    if(shelf.GetItemCount() <= 0) continue;
                    
                    // Apply stat
                    totalStrRate += itemDataSo.strBonus;
                    totalAgiRate += itemDataSo.agiBonus;
                    totalStaRate += itemDataSo.staBonus;
                    totalSleepRate += itemDataSo.GetSleepBonus();
                    totalHungerRate += itemDataSo.GetHungerBonus();
                }
            }
            
            // Check if not shelf will no duplicate placement
            if (!hasShelf && equip.LinkedData != null)
            {
                // No place duplicated item
                if(addedItems.Contains(equip.LinkedData)) continue;
                addedItems.Add(equip.LinkedData);
            }
            
            totalStrRate += equip.strPerSecond;
            totalAgiRate += equip.agiPerSecond;
            totalStaRate += equip.staPerSecond;
            totalSleepRate += equip.sleepPerSecond;
            totalHungerRate += equip.hungerPerSecond;
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
        RecalculateRates();
    }

    public void UnlockRoom()
    {
        IsUnlocked = true;
        OnRoomUnlocked?.Invoke(this);
    }
}