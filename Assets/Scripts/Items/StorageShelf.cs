using UnityEngine;
using System.Collections.Generic;

public class StorageShelf : MonoBehaviour, IInteractable
{
    [Header("Settings")]
    public ItemDataSO allowedItem; 
    public Transform[] spawnPoints;
    
    [SerializeField] private float stockSpeed = 0.15f;
    private float lastStockTime; 

    private List<GameObject> stockedItems = new List<GameObject>(); 

    public string GetInteractionPrompt()
    {
        return "Hold Left Click to Stock / Right Click to Retrieve";
    }

    public void OnInteract(PlayerInteraction player)
    {
        // ... (Same logic as before for Left Click) ...
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
        // --- RETRIEVE LOGIC (Right Click Hold) ---

        // 1. Check Cooldown
        if (Time.time < lastStockTime + stockSpeed) return;

        // 2. Check if Shelf has items to give
        if (stockedItems.Count == 0) return;

        // 3. Check what the player is holding
        GameObject heldObject = player.GetHeldObject();
        if (heldObject != null)
        {
            ItemBox box = heldObject.GetComponent<ItemBox>();

            // 4. Does the player have a Valid Box?
            if (box != null && box.itemData == allowedItem)
            {
                // 5. Try to Add item to Box
                if (box.TryAddItem())
                {
                    RemoveVisualItem(); // Visual logic
                    lastStockTime = Time.time;
                }
                else
                {
                    Debug.Log("Box is Full!");
                }
            }
        }
        else
        {
            // Optional: If hands are empty, maybe pick up the single item directly?
            // (You can implement this later if you want single-item carrying)
        }
    }

    private void AddVisualItem()
    {
        int index = stockedItems.Count;
        Transform spot = spawnPoints[index];
        GameObject newItem = Instantiate(allowedItem.itemPrefab, spot.position, spot.rotation);
        newItem.transform.SetParent(this.transform); 
        stockedItems.Add(newItem);
    }

    private void RemoveVisualItem()
    {
        int lastIndex = stockedItems.Count - 1;
        GameObject itemToRemove = stockedItems[lastIndex];
        Destroy(itemToRemove);
        stockedItems.RemoveAt(lastIndex);
    }
}