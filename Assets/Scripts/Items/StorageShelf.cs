using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Systems;
using UI;
using Utilities;

public class StorageShelf : BaseInteractable
{
    [Header("Settings")]
    public ItemDataSO[] allowedItems; 
    public Transform[] spawnPoints;
    public GameObject itemBoxPrefab; // REFERENCE TO THE CARDBOARD BOX PREFAB
    
    [SerializeField] private float stockSpeed = 0.15f;
    private float lastStockTime; 

    private List<GameObject> stockedItems = new List<GameObject>();
    public ItemDataSO activeItem;
    public event Action<StorageShelf> OnShelfItemAdd;
    public event Action<StorageShelf> OnShelfItemRemove;

    private float _consumeTime;
    private bool _isHandEmpty = true;
    
    protected override void Start()
    {
        base.Start();
        GameManager.Instance.GameTimeManager.OnGameMinuteTick += OnGameMinuteTick;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        GameManager.Instance.GameTimeManager.OnGameMinuteTick -= OnGameMinuteTick;
    }

    private void OnGameMinuteTick(string obj)
    {        
        if(!activeItem) return;
        if(stockedItems.Count <= 0) return;
        _consumeTime += 1f;
        if(_consumeTime <= activeItem.consumeTimeTick) return;
        _consumeTime = 0f;
        RemoveVisualItem(default);
    }

    protected override void InitInteractionPrompts()
    {
        AddInteractionPrompt(new []
        {
            new InteractionPromptData
            {
                Icon = KeyIcon.MOUSE_LEFT_CLICK,
                Prompt = "(Hold) STOCK"
            },
            new InteractionPromptData
            {
                Icon = KeyIcon.MOUSE_RIGHT_CLICK,
                Prompt = "(Hold) RETRIEVE"
            }
        });
    }

    public override void OnInteract(PlayerInteraction player)
    {
        // ... (Existing Stocking Logic - unchanged) ...
        _isHandEmpty = player.GetHeldObject() == null;
        if (Time.time < lastStockTime + stockSpeed) return;
        if (_isHandEmpty) return;

        ItemBox box = player.GetHeldObject().GetComponent<ItemBox>();

        if (activeItem && activeItem != box.itemData)
        {
            return;
        }
        
        if (box != null &&  allowedItems.Contains(box.itemData) && stockedItems.Count < spawnPoints.Length)
        {
            if (box.TryTakeItem())
            {
                activeItem = box.itemData;
                AddVisualItem(box.itemData, box.transform.position);
                lastStockTime = Time.time;
            }
        }
    }

    public void SetItemStorage(ItemDataSO itemDataSo, int amount)
    {
        activeItem = itemDataSo;
        for (int i = 0; i < amount; i++)
        {
            AddVisualItem(itemDataSo, Vector3.zero);
        }
    }

    public override void OnAltInteract(PlayerInteraction player)
    {
        // --- RETRIEVE LOGIC (Right Click) ---

        // 1. Check Cooldown & Availability
        if (Time.time < lastStockTime + stockSpeed) return;
        
        if (stockedItems.Count == 0)
        {
            return; // Nothing to take
        }

        GameObject heldObject = player.GetHeldObject();

        // SCENARIO A: Player is holding a Box
        if (heldObject != null)
        {
            ItemBox box = heldObject.GetComponent<ItemBox>();

            // Check if box matches item
            if (box != null && box.itemData == activeItem)
            {
                if (box.TryAddItem())
                {
                    RemoveVisualItem(box.transform);
                    lastStockTime = Time.time;
                }
            }
        }
        // SCENARIO B: Player hands are EMPTY (Auto-Spawn Box)
        else
        {
            // 1. Spawn a new Box at the player's hand position (temp)
            GameObject newBoxObj = Instantiate(itemBoxPrefab, player.holdPoint.position, Quaternion.identity);
            ItemBox newBox = newBoxObj.GetComponent<ItemBox>();

            // 2. Configure the Box
            if (newBox != null)
            {
                newBox.itemData = activeItem;
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
            RemoveVisualItem(newBox.transform);
            lastStockTime = Time.time;
        }
    }

    // ... (AddVisualItem and RemoveVisualItem methods remain the same) ...
    private void AddVisualItem(ItemDataSO itemDataSo, Vector3 from)
    {
        int index = stockedItems.Count;
        Transform spot = spawnPoints[index];
        GameObject newItem = Instantiate(activeItem.itemPrefab, spot.position, spot.rotation);
        newItem.transform.SetParent(this.transform);
        stockedItems.Add(newItem);
        OnShelfItemAdd?.Invoke(this);
  
        if (from != Vector3.zero)
        {
            newItem.SetActive(false);
            itemDataSo.AddStockAnimated(from, spot, () =>
            {
                newItem.SetActive(true);
            });
        }
    }

    private void RemoveVisualItem(Transform toSpot)
    {
        int lastIndex = stockedItems.Count - 1;
        GameObject itemToRemove = stockedItems[lastIndex];
        Vector3 removePosition = itemToRemove.transform.position;
        Destroy(itemToRemove);
        stockedItems.RemoveAt(lastIndex);

        #region VISUAL
        if (toSpot != null)
        {
            activeItem.AddStockAnimated(removePosition, toSpot, default);
        }
        #endregion
        
        if (stockedItems.Count == 0)
        {
            activeItem = null;
            _consumeTime = 0;
        }
        
        OnShelfItemRemove?.Invoke(this);
        
    }
    
    // --- NEW: AI API ---
    
    public bool HasConsumable()
    {
        if (activeItem == null || !activeItem.isConsumable) return false;
        return stockedItems.Count > 0;
    }

    public ItemDataSO GetItem()
    {
        return activeItem;
    }
    
    public int GetItemCount()
    {
        return stockedItems.Count;
    }
}