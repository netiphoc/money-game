using System;
using System.Collections.Generic;
using Systems;
using UI;
using UnityEngine;
using Utilities;

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

    public event Action<ItemBox> OnItemChanged;
    
    protected override void Awake()
    {
        base.Awake();
        
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    protected override void InitInteractionPrompts()
    {
        AddInteractionPrompt(new []
        {
            InteractionPrompt_PLACE,
            InteractionPrompt_THROW
        });
        
        AddInteractionPrompt(new []
        {
            new InteractionPromptData
            {
                Icon = KeyIcon.ALERT,
                RealTimePrompt = GetItemInfoPrompt
            }
        });
        
        AddInteractionPrompt(new []
        {
            InteractionPrompt_PICKUP,
            new InteractionPromptData
            {
                Icon = KeyIcon.X,
                RealTimePrompt = GetItemSellInfoPrompt
            },
        });
    }

    private string GetItemInfoPrompt()
    {
        return $"{itemData.itemName} x{currentQuantity}";
    }

    private string GetItemSellInfoPrompt()
    {
        return $"SELL {itemData.itemName} x{currentQuantity} for {GetWorth().ToMoneyFormat()}";
    }
    
    protected override void OnDestroy()
    {
        base.OnDestroy();
        
        foreach (var t in currentRooms)
        {
            t.RemoveItemBox(this);
        }
    }

    public override InteractionPromptData[] GetInteractionPrompts()
    {
        if (isHeld) 
        {
            // If it's furniture, tell them clicking will place it
            if (itemData.placementData != null) return GetInteractionPromptByIndex(0);
            
            // If it's regular shelf items (e.g. Water)
            return GetInteractionPromptByIndex(1);
        } 
        
        return GetInteractionPromptByIndex(2);
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
    
    public void Sell(PlayerInteraction player)
    {
        Throw(player);

        GameManager.Instance.AddMoney(GetWorth());
        FloatingTextManager.Instance.ShowMoneyText(transform.position,GetWorth());
        
        Destroy(gameObject);
    }

    // --- HELPER FOR PLACEMENT MANAGER ---
    public bool TryTakeItem()
    {
        if (currentQuantity > 0)
        {
            currentQuantity--;
            OnItemChanged?.Invoke(this);

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
            OnItemChanged?.Invoke(this);
            return true;
        }
        return false;
    }

    public int GetWorth()
    {
        return itemData.cost * currentQuantity;
    }
}