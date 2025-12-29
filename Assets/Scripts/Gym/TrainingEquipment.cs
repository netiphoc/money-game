using System;
using System.Collections.Generic;
using UnityEngine;

public class TrainingEquipment : MonoBehaviour
{
    [Header("Idle Production (Per Second)")]
    public float strPerSecond;
    public float agiPerSecond;
    public float staPerSecond;
    [field: SerializeField, Header("Consumable")] public StorageShelf[] StorageShelfShelves { get; private set; }

    [Header("Visuals")]
    public string animationTrigger; 
    public Transform interactionPoint;
    
    // NEW: Track which room owns this equipment
    public List<GymRoom> currentRooms = new List<GymRoom>();

    public event Action<StorageShelf> OnStorageShelfUpdated;
    
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

    private void OnShelfItemAdd(StorageShelf storageShelf)
    {
        OnStorageShelfUpdated?.Invoke(storageShelf);
    }

    private void OnShelfItemRemove(StorageShelf storageShelf)
    {
        OnStorageShelfUpdated?.Invoke(storageShelf);
    }

    // --- CLEANUP LOGIC ---
    private void OnDestroy()
    {
        foreach (var t in currentRooms)
        {
            t.RemoveEquipment(this);
        }
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
}