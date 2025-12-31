using System;
using UnityEngine;
using System.Collections.Generic;
using Data;
using SaveLoadSystem;
using UnityEngine.InputSystem;

public class GymRoom : MonoBehaviour, ISaveLoadSystem
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
    
    [Serializable]
    public class TrainingEquipmentData
    {
        public string itemId;
        public Vector3 position;
        public Quaternion rotation;
        
        // Storage
        public string[] storageIds;
        public int[] storageAmount;
    }
    
    [Serializable]
    public class TrainingEquipmentGroupData
    {
        public TrainingEquipmentData[] data;
    }

    public void SaveGame()
    {
        // Unlock
        PlayerPrefs.SetInt($"{name}_IsUnlocked", IsUnlocked ? 1 : 0);

        if (assignedBoxer)
        {
            PlayerPrefs.SetString($"{name}_boxerName", assignedBoxer.stats.boxerName);
            PlayerPrefs.SetFloat($"{name}_strength", assignedBoxer.stats.strength);
            PlayerPrefs.SetFloat($"{name}_agility", assignedBoxer.stats.agility);
            PlayerPrefs.SetFloat($"{name}_stamina", assignedBoxer.stats.stamina);
            PlayerPrefs.SetInt($"{name}_level", assignedBoxer.stats.level);
            PlayerPrefs.SetFloat($"{name}_currentXP", assignedBoxer.stats.currentXP);
            PlayerPrefs.SetFloat($"{name}_xpToNextLevel", assignedBoxer.stats.xpToNextLevel);
        }
        
        // Equipment
        List<TrainingEquipmentData> equipmentData = new List<TrainingEquipmentData>();
            
        foreach (var trainingEquipment in equipmentInRoom)
        {
            ItemDataSO itemDataSo = trainingEquipment.LinkedData.linkedItemData;
            List<string> storageIds = new List<string>(); 
            List<int> storageAmount = new List<int>();

            foreach (var storageShelf in trainingEquipment.StorageShelfShelves)
            {
                if (storageShelf.activeItem == null || storageShelf.GetItemCount() == 0)
                {
                    storageIds.Add("null");
                    storageAmount.Add(0);
                    continue;
                }
                
                storageIds.Add(storageShelf.activeItem.itemName);
                storageAmount.Add(storageShelf.GetItemCount());
            }
            
            equipmentData.Add(new TrainingEquipmentData
            {
                itemId = itemDataSo.itemName,
                position = trainingEquipment.transform.position,
                rotation = trainingEquipment.transform.rotation,
                storageIds = storageIds.ToArray(),
                storageAmount = storageAmount.ToArray(),
            });
        }

        string equipmentJson = JsonUtility.ToJson(new TrainingEquipmentGroupData
        {
            data = equipmentData.ToArray(),
        });

        PlayerPrefs.SetString($"{name}_equipmentInRoom", equipmentJson);
    }

    public void LoadGame()
    {
        // Unlock
        bool isUnlock = PlayerPrefs.GetInt($"{name}_IsUnlocked", 0) == 1;
        if (isUnlock) UnlockRoom();

        // Boxer
        if (PlayerPrefs.HasKey($"{name}_boxerName"))
        {
            BoxerData boxerData = new BoxerData();
            boxerData.InitStats();
            
            boxerData.boxerName = PlayerPrefs.GetString($"{name}_boxerName");
            boxerData.strength = PlayerPrefs.GetFloat($"{name}_strength");
            boxerData.agility = PlayerPrefs.GetFloat($"{name}_agility");
            boxerData.stamina = PlayerPrefs.GetFloat($"{name}_stamina");
            boxerData.level = PlayerPrefs.GetInt($"{name}_level");
            boxerData.currentXP = PlayerPrefs.GetFloat($"{name}_currentXP");
            boxerData.xpToNextLevel = PlayerPrefs.GetFloat($"{name}_xpToNextLevel");
            
            BoxerController boxerController = SaveSystem.Instance.LoadBoxerData(boxerData, SpawnPoint);
            assignedBoxer = boxerController;
            boxerController.assignedRoom = this;
        }
        
        // Equipment
        if (PlayerPrefs.HasKey($"{name}_equipmentInRoom"))
        {
          string json = PlayerPrefs.GetString($"{name}_equipmentInRoom");
          TrainingEquipmentGroupData data = JsonUtility.FromJson<TrainingEquipmentGroupData>(json);

          foreach (var trainingEquipment in data.data)
          {
              ItemDataSO item = SaveSystem.Instance.GetItemDataFromItemName(trainingEquipment.itemId);
              if(item == default) continue;
              GameObject obj = Instantiate(item.itemPrefab, trainingEquipment.position, trainingEquipment.rotation);
              
              // Save storage items
              TrainingEquipment equipment = obj.GetComponent<TrainingEquipment>();
              if (equipment == null) continue;
              if(equipment.StorageShelfShelves.Length != trainingEquipment.storageIds.Length) continue;
              if(equipment.StorageShelfShelves.Length != trainingEquipment.storageAmount.Length) continue;

              for (int i = 0; i < equipment.StorageShelfShelves.Length; i++)
              {
                  if(trainingEquipment.storageIds.Equals("null")) continue;
                  ItemDataSO itemDataSo = SaveSystem.Instance.GetItemDataFromItemName(trainingEquipment.storageIds[i]);
                  if(itemDataSo == default) continue;
                  StorageShelf storageShelf = equipment.StorageShelfShelves[i];
                  storageShelf.SetItemStorage(itemDataSo, trainingEquipment.storageAmount[i]);
              }
          }
        }
    }
}