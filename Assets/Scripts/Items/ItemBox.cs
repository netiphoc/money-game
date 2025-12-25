using UnityEngine;

public class ItemBox : MonoBehaviour, IInteractable
{
    [Header("Box Data")]
    public ItemDataSO itemData;
    public int currentQuantity = 1; // Default to 1 for furniture boxes
    public int maxCapacity = 1;     // Furniture boxes usually hold 1 item

    private Rigidbody rb;
    private Collider col;
    private bool isHeld = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    public string GetInteractionPrompt()
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

    public void OnInteract(PlayerInteraction player)
    {
        // 1. PICK UP LOGIC (If not already holding it)
        if (!isHeld)
        {
            isHeld = true;
            rb.isKinematic = true;
            col.enabled = false;
            player.AttachToHand(this.gameObject);
            return;
        }

        // 2. WHILE HELD LOGIC
        if (isHeld)
        {
            // A. FURNITURE (Placement Mode) -> INSTANT CLICK
            if (itemData.placementData != null && currentQuantity > 0)
            {
                // We check for a single click frame, not a hold
                if (player.WasPrimaryPressed()) 
                {
                    PlacementManager.Instance.StartPlacementFromBox(itemData.placementData, this);
                }
            }
            // B. SHELF ITEMS (Soda/Food) -> HOLD TO STOCK
            else
            {
                // This logic is handled by the Shelf script looking at the player, 
                // so we don't need to do anything here.
            }
        }
    }

    public void OnAltInteract(PlayerInteraction player) 
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