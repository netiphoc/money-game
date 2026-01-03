using System;
using UnityEngine;
using System.Collections.Generic;
using Data;
using SaveLoadSystem;

public class GymRoom : MonoBehaviour, ISaveLoadSystem
{
    [SerializeField] private GymRoom[] linkGymRooms;

    [field: SerializeField] public Transform SpawnPoint { get; private set; } 
    [field: SerializeField] public RoomDataSO RoomDataSo { get; private set; } 

    [Header("Room Info")]
    public BoxerController assignedBoxer;
    public List<TrainingEquipment> equipmentInRoom = new List<TrainingEquipment>();
    public List<ItemBox> itemBoxInRoom = new List<ItemBox>();

    [Header("Capacity Limit")]
    public int maxCapacity = 10; // Default limit
    public int capacityLevel = 1;
    
    [Header("Live Production Rates (Read Only)")]
    public float totalStrRate;
    public float totalAgiRate;
    public float totalStaRate;
    public float totalHungerRate;
    public float totalSleepRate;
    public event Action<GymRoom> OnRoomUnlocked;
    public bool IsUnlocked { get; private set; }
    
    private readonly List<TrainingEquipment> _trainingEquipments = new List<TrainingEquipment>();
    
    public IEnumerable<TrainingEquipment> GetPermitEquipments(BoxerController boxer, bool includeConsumable = true)
    {
        _trainingEquipments.Clear();
        _trainingEquipments.AddRange(equipmentInRoom);
        
        foreach (var gymRoom in linkGymRooms)
        {
            _trainingEquipments.AddRange(gymRoom.equipmentInRoom);
        }
        
        foreach (var trainingEquipment in _trainingEquipments)
        {
            if(boxer == null) break; 
            if(trainingEquipment.LinkedData.linkedItemData.requiredBoxerLevel > boxer.stats.level) continue;
            if(!includeConsumable && trainingEquipment.IsConsumable()) continue;
            yield return trainingEquipment;
        }
    }

    public bool CanFitMore()
    {
        return equipmentInRoom.Count < maxCapacity;
    }

    public void UpgradeCapacity()
    {
        capacityLevel++;
        maxCapacity += 5; // Add 5 slots per upgrade
    }

    public int GetUpgradeCost()
    {
        return 500 * capacityLevel; // Example: $500, $1000, $1500...
    }
    
    private void RecalculateRates()
    {
        if(assignedBoxer == null) return;
        
        totalStrRate = 0;
        totalAgiRate = 0;
        totalStaRate = 0;
        totalSleepRate = 0;
        totalHungerRate = 0;

        List<PlaceableDataSO> addedItems = new List<PlaceableDataSO>();
        foreach (var equip in GetPermitEquipments(assignedBoxer))
        {
            bool hasShelf = equip.TryGetConsumableShelf(out StorageShelf[] shelves);
            
            // Check consume able
            if (hasShelf)
            {
                foreach (var shelf in shelves)
                {
                    ItemDataSO itemDataSo = shelf.GetItem();
                    
                    if(shelf.GetItemCount() <= 0) continue;
                    
                    // Apply stat
                    totalStrRate += itemDataSo.strBonus;
                    totalAgiRate += itemDataSo.agiBonus;
                    totalStaRate += itemDataSo.staBonus;
                    totalSleepRate += itemDataSo.sleepBonus;
                    totalHungerRate += itemDataSo.hungerBonus;
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
    
    public void RemoveEquipment(TrainingEquipment equip)
    {
        if (equipmentInRoom.Contains(equip))
        {
            equip.OnStorageShelfUpdated -= OnStorageShelfUpdated;
            equipmentInRoom.Remove(equip);
            RecalculateRates();
        }
    }
    public void RemoveItemBox(ItemBox itemBox)
    {
        if (itemBoxInRoom.Contains(itemBox))
        {
            itemBoxInRoom.Remove(itemBox);
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
        
        ItemBox itemBox = other.GetComponent<ItemBox>();
        if (itemBox != null && !itemBoxInRoom.Contains(itemBox))
        {
            itemBoxInRoom.Add(itemBox);
            itemBox.currentRooms.Add(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        TrainingEquipment equip = other.GetComponent<TrainingEquipment>();
        if (equip != null && equipmentInRoom.Contains(equip))
        {
            equip.OnStorageShelfUpdated -= OnStorageShelfUpdated;
            equipmentInRoom.Remove(equip);
            equip.currentRooms.Remove(this);
            RecalculateRates();
        }
        
        ItemBox itemBox = other.GetComponent<ItemBox>();
        if (itemBox != null && itemBoxInRoom.Contains(itemBox))
        {
            itemBoxInRoom.Remove(itemBox);
            itemBox.currentRooms.Remove(this);
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
    
    [Serializable]
    public class ItemBoxData
    {
        public string itemId;
        public int currentQuantity;
        public Vector3 position;
        public Quaternion rotation;
    }
    
    [Serializable]
    public class ItemBoxGroupData
    {
        public ItemBoxData[] data;
    }

    public void SaveGame()
    {
        #region Gym Stats
        // Unlock
        PlayerPrefs.SetInt($"{name}_IsUnlocked", IsUnlocked ? 1 : 0);

        // Stats
        if (assignedBoxer)
        {
            PlayerPrefs.SetString($"{name}_boxerName", assignedBoxer.stats.boxerName);
            PlayerPrefs.SetFloat($"{name}_strength", assignedBoxer.stats.strength);
            PlayerPrefs.SetFloat($"{name}_agility", assignedBoxer.stats.agility);
            PlayerPrefs.SetFloat($"{name}_stamina", assignedBoxer.stats.stamina);
            PlayerPrefs.SetFloat($"{name}_hunger", assignedBoxer.stats.hunger);
            PlayerPrefs.SetInt($"{name}_level", assignedBoxer.stats.level);
            PlayerPrefs.SetFloat($"{name}_currentXP", assignedBoxer.stats.currentXP);
            PlayerPrefs.SetFloat($"{name}_xpToNextLevel", assignedBoxer.stats.xpToNextLevel);
        }
        

        #endregion
        
        #region Equipment Data

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
        

        #endregion
        
        #region Item Box Data

        List<ItemBoxData> itemBoxData = new List<ItemBoxData>();
        foreach (var itemBox in itemBoxInRoom)
        {
            ItemDataSO itemDataSo = itemBox.itemData;
            itemBoxData.Add(new ItemBoxData
            {
                itemId = itemDataSo.itemName,
                currentQuantity = itemBox.currentQuantity,
                position = itemBox.transform.position,
                rotation = itemBox.transform.rotation,
            });
        }

        string itemBoxJson = JsonUtility.ToJson(new ItemBoxGroupData()
        {
            data = itemBoxData.ToArray(),
        });

        PlayerPrefs.SetString($"{name}_itemBoxInRoom", itemBoxJson);
        

        #endregion
    }

    public void LoadGame()
    {
        #region Gym Data
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
            boxerData.hunger = PlayerPrefs.GetFloat($"{name}_hunger");
            boxerData.level = PlayerPrefs.GetInt($"{name}_level");
            boxerData.currentXP = PlayerPrefs.GetFloat($"{name}_currentXP");
            boxerData.xpToNextLevel = PlayerPrefs.GetFloat($"{name}_xpToNextLevel");
            boxerData.UpdateTotal();
            
            BoxerController boxerController = SaveSystem.Instance.LoadBoxerData(boxerData, SpawnPoint);
            assignedBoxer = boxerController;
            boxerController.assignedRoom = this;
        }

        #endregion

        #region Equipment Data

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

        #endregion

        #region Item Box Data

        if (PlayerPrefs.HasKey($"{name}_itemBoxInRoom"))
        {
            string json = PlayerPrefs.GetString($"{name}_itemBoxInRoom");
            ItemBoxGroupData data = JsonUtility.FromJson<ItemBoxGroupData>(json);

            foreach (var itemBoxData in data.data)
            {
                ItemDataSO item = SaveSystem.Instance.GetItemDataFromItemName(itemBoxData.itemId);
                if(item == default) continue;
                ItemBox itemBox = SaveSystem.Instance.GetItemBox();
                itemBox.transform.position = itemBoxData.position;
                itemBox.transform.rotation = itemBoxData.rotation;
                itemBox.itemData = item;
                itemBox.currentQuantity = itemBoxData.currentQuantity;
            }
        }

        #endregion
    }
}