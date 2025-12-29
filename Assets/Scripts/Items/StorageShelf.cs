using System;
using UnityEngine;
using System.Collections.Generic;

public class StorageShelf : MonoBehaviour, IInteractable
{
    [Header("Settings")]
    public ItemDataSO allowedItem; 
    public Transform[] spawnPoints;
    public GameObject itemBoxPrefab; // REFERENCE TO THE CARDBOARD BOX PREFAB
    
    [SerializeField] private float stockSpeed = 0.15f;
    private float lastStockTime; 

    private List<GameObject> stockedItems = new List<GameObject>();

    public event Action<StorageShelf> OnShelfItemAdd;
    public event Action<StorageShelf> OnShelfItemRemove;

    private float _consumeTime;

    private void Start()
    {
        GameManager.Instance.GameTimeManager.OnGameMinuteTick += OnGameMinuteTick;
    }

    private void OnDestroy()
    {
        GameManager.Instance.GameTimeManager.OnGameMinuteTick -= OnGameMinuteTick;
    }

    private void OnGameMinuteTick(string obj)
    {        
        if(stockedItems.Count <= 0) return;
        _consumeTime += 1f;
        if(_consumeTime < allowedItem.consumeTimeTick) return;
        _consumeTime = 0f;
        RemoveVisualItem();
    }

    public string GetInteractionPrompt()
    {
        return "Hold LMB to Stock / RMB to Retrieve";
    }

    public void OnInteract(PlayerInteraction player)
    {
        // ... (Existing Stocking Logic - unchanged) ...
        if (Time.time < lastStockTime + stockSpeed) return;
        if (player.GetHeldObject() == null) return;

        ItemBox box = player.GetHeldObject().GetComponent<ItemBox>();

        if (box != null && box.itemData == allowedItem && stockedItems.Count < spawnPoints.Length)
        {
            if (box.TryTakeItem())
            {
                AddVisualItem();
                lastStockTime = Time.time;
            }
        }
    }

    public void OnAltInteract(PlayerInteraction player)
    {
        // --- RETRIEVE LOGIC (Right Click) ---

        // 1. Check Cooldown & Availability
        if (Time.time < lastStockTime + stockSpeed) return;
        
        if (stockedItems.Count == 0)
        {
            Debug.Log("stockedItems.Count == 0");
            return; // Nothing to take
        }

        GameObject heldObject = player.GetHeldObject();

        // SCENARIO A: Player is holding a Box
        if (heldObject != null)
        {
            Debug.Log("Player is holding a Box");
            ItemBox box = heldObject.GetComponent<ItemBox>();

            // Check if box matches item
            if (box != null && box.itemData == allowedItem)
            {
                if (box.TryAddItem())
                {
                    RemoveVisualItem();
                    lastStockTime = Time.time;
                }
            }
        }
        // SCENARIO B: Player hands are EMPTY (Auto-Spawn Box)
        else
        {
            Debug.Log("Player hands are EMPTY (Auto-Spawn Box)");
            // 1. Spawn a new Box at the player's hand position (temp)
            GameObject newBoxObj = Instantiate(itemBoxPrefab, player.holdPoint.position, Quaternion.identity);
            ItemBox newBox = newBoxObj.GetComponent<ItemBox>();

            // 2. Configure the Box
            if (newBox != null)
            {
                newBox.itemData = allowedItem;
                newBox.currentQuantity = 1; // Start with the 1 item we just took
            }
            else
            {
                Debug.Log("Item Box NULL");
            }

            // 3. Attach to Player Hand
            // We call OnInteract directly to trigger the pickup logic (physics disable, parenting)
            newBox.OnInteract(player);

            // 4. Remove the item from the shelf
            RemoveVisualItem();
            lastStockTime = Time.time;
        }
    }

    // ... (AddVisualItem and RemoveVisualItem methods remain the same) ...
    private void AddVisualItem()
    {
        int index = stockedItems.Count;
        Transform spot = spawnPoints[index];
        GameObject newItem = Instantiate(allowedItem.itemPrefab, spot.position, spot.rotation);
        newItem.transform.SetParent(this.transform); 
        stockedItems.Add(newItem);
        OnShelfItemAdd?.Invoke(this);
    }

    private void RemoveVisualItem()
    {
        int lastIndex = stockedItems.Count - 1;
        GameObject itemToRemove = stockedItems[lastIndex];
        Destroy(itemToRemove);
        stockedItems.RemoveAt(lastIndex);

        if (stockedItems.Count == 0)
        {
            _consumeTime = 0;
        }
        
        OnShelfItemRemove?.Invoke(this);
    }
    
    // --- NEW: AI API ---
    
    public bool HasConsumable()
    {
        if (allowedItem == null || !allowedItem.isConsumable) return false;
        return stockedItems.Count > 0;
    }

    public ItemDataSO GetItem()
    {
        return allowedItem;
    }
    
    public int GetItemCount()
    {
        return stockedItems.Count;
    }
}