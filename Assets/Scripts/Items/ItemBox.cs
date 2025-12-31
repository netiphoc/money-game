using System.Collections.Generic;
using Systems;
using UnityEngine;

public class ItemBox : BaseInteractable
{
    [Header("Box Data")]
    public ItemDataSO itemData;
    public int currentQuantity = 1; // Default to 1 for furniture boxes
    public int maxCapacity = 1;     // Furniture boxes usually hold 1 item

    private Rigidbody rb;
    private Collider col;
    private bool isHeld = false;

    // NEW: Track which room owns this equipment
    public List<GymRoom> currentRooms = new List<GymRoom>();
    
    protected override void Awake()
    {
        base.Awake();
        
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        
        foreach (var t in currentRooms)
        {
            t.RemoveItemBox(this);
        }
    }

    public override string GetInteractionPrompt()
    {
        if (isHeld) 
        {
            // If it's furniture, tell them clicking will place it
            if (itemData.placementData != null) return "Click LMB to Place / G to Throw";
            
            // If it's regular shelf items (e.g. Water)
            return "Hold LMB to Stock / G to Throw"; 
        }
        return $"Click LMB {itemData.itemName} Box";
    }

    public override void OnInteract(PlayerInteraction player)
    {
        // 1. PICK UP LOGIC (If box is on the floor/shelf)
        if (!isHeld)
        {
            // FIX: Only pick up if the player's hands are EMPTY
            if (player.GetHeldObject() == null)
            {
                isHeld = true;
                rb.isKinematic = true;
                col.enabled = false;
                player.AttachToHand(this.gameObject);
            }
            return;
        }

        // 2. WHILE HELD LOGIC (Interact with the box itself while holding it)
        if (isHeld)
        {
            // A. FURNITURE -> INSTANT CLICK TO START PLACEMENT
            if (itemData.placementData != null && currentQuantity > 0)
            {
                if (player.WasPrimaryPressed()) 
                {
                    PlacementManager.Instance.StartPlacementFromBox(itemData.placementData, this);
                }
            }
            // B. SHELF ITEMS -> (Logic handled by Shelf script, do nothing here)
        }
    }

    public override void OnAltInteract(PlayerInteraction player) 
    { 
        // Right click logic (handled by Shelf usually)
    }

    public void Throw(PlayerInteraction player)
    {
        if (isHeld)
        {
            isHeld = false;
            player.ReleaseFromHand();
            col.enabled = true;
            rb.isKinematic = false;
            rb.AddForce(player.playerCamera.transform.forward * 600f);
        }
    }

    // --- HELPER FOR PLACEMENT MANAGER ---
    public bool TryTakeItem()
    {
        if (currentQuantity > 0)
        {
            currentQuantity--;
            
            // If box is empty, destroy it?
            if (currentQuantity <= 0)
            {
                Destroy(gameObject); // Auto-destroy box when empty (optional)
            }
            return true;
        }
        return false;
    }

    public bool TryAddItem()
    {
        if (currentQuantity < maxCapacity)
        {
            currentQuantity++;
            return true;
        }
        return false;
    }
}