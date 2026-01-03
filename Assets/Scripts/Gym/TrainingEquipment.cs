using System;
using System.Collections.Generic;
using SaveLoadSystem;
using UnityEngine;

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


public class TrainingEquipment : MonoBehaviour, ISaveLoadSystem
{
    [Header("Save")]
    public bool isPreInstalled;
    
    [Header("Idle Production (Per Second)")]
    public float strPerSecond;
    public float agiPerSecond;
    public float staPerSecond;
    
    [Header("Idle Survival (Per Second)")]
    public float hungerPerSecond;
    public float sleepPerSecond;
    
    [field: SerializeField, Header("Consumable")] public StorageShelf[] StorageShelfShelves { get; private set; }

    [Header("Visuals")]
    public string animationTrigger; 
    public Transform interactionPoint;
    
    // NEW: Track which room owns this equipment
    public List<GymRoom> currentRooms = new List<GymRoom>();

    public event Action<StorageShelf> OnStorageShelfUpdated;
    public PlaceableDataSO LinkedData;

    private void Awake()
    {
        LinkedData = GetComponent<MoveableFurniture>().data;
    }

    private void OnEnable()
    {
        foreach (var storageShelf in StorageShelfShelves)
        {
            storageShelf.OnShelfItemAdd += OnShelfItemAdd;
            storageShelf.OnShelfItemRemove += OnShelfItemRemove;
        }
    }

    private void OnDisable()
    {
        foreach (var storageShelf in StorageShelfShelves)
        {
            storageShelf.OnShelfItemAdd -= OnShelfItemAdd;
            storageShelf.OnShelfItemRemove -= OnShelfItemRemove;
        }
    }
    
    private void OnDestroy()
    {
        foreach (var t in currentRooms)
        {
            t.RemoveEquipment(this);
        }
    }
    
    private void OnShelfItemAdd(StorageShelf storageShelf)
    {
        OnStorageShelfUpdated?.Invoke(storageShelf);
    }

    private void OnShelfItemRemove(StorageShelf storageShelf)
    {
        OnStorageShelfUpdated?.Invoke(storageShelf);
    }

    public bool IsConsumable()
    {
        return StorageShelfShelves.Length > 0;
    }

    public bool TryGetConsumableShelf(out StorageShelf[] storageShelfShelves)
    {
        List<StorageShelf> shelves = new List<StorageShelf>();
        foreach (var shelfShelf in StorageShelfShelves)
        {
            if(!shelfShelf.HasConsumable()) continue;
            shelves.Add(shelfShelf);
        }

        storageShelfShelves = shelves.ToArray();
        return shelves.Count > 0;
    }

    private string GetSaveKey()
    {
        return $"{name}{transform.position}";
    }

    public void SaveGame()
    {
        if(!isPreInstalled) return;
        ItemDataSO itemDataSo = LinkedData.linkedItemData;
        List<string> storageIds = new List<string>(); 
        List<int> storageAmount = new List<int>();

        foreach (var storageShelf in StorageShelfShelves)
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

        TrainingEquipmentData trainingEquipmentData = new TrainingEquipmentData
        {
            itemId = itemDataSo.itemName,
            position = transform.position,
            rotation = transform.rotation,
            storageIds = storageIds.ToArray(),
            storageAmount = storageAmount.ToArray(),
        };
        
        string equipmentJson = JsonUtility.ToJson(trainingEquipmentData);
        PlayerPrefs.SetString($"{GetSaveKey()}_equipment", equipmentJson);
    }

    public void LoadGame()
    {
        if(!isPreInstalled) return;
        if (PlayerPrefs.HasKey($"{GetSaveKey()}_equipment"))
        {
            string json = PlayerPrefs.GetString($"{GetSaveKey()}_equipment");
            TrainingEquipmentData data = JsonUtility.FromJson<TrainingEquipmentData>(json);

            ItemDataSO item = SaveSystem.Instance.GetItemDataFromItemName(data.itemId);
            if(item == default) return;
            GameObject obj = Instantiate(item.itemPrefab, data.position, data.rotation);
              
            // Save storage items
            TrainingEquipment equipment = obj.GetComponent<TrainingEquipment>();
            if (equipment == null) return;
            if(equipment.StorageShelfShelves.Length != data.storageIds.Length) return;
            if(equipment.StorageShelfShelves.Length != data.storageAmount.Length) return;

            for (int i = 0; i < equipment.StorageShelfShelves.Length; i++)
            {
                if(data.storageIds.Equals("null")) continue;
                ItemDataSO itemDataSo = SaveSystem.Instance.GetItemDataFromItemName(data.storageIds[i]);
                if(itemDataSo == default) continue;
                StorageShelf storageShelf = equipment.StorageShelfShelves[i];
                storageShelf.SetItemStorage(itemDataSo, data.storageAmount[i]);
            }
            
            Destroy(gameObject);
        }
        
    }
}