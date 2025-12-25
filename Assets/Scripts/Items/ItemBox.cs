using UnityEngine;

public class ItemBox : MonoBehaviour, IInteractable
{
    [Header("Box Data")]
    public ItemDataSO itemData;
    public int currentQuantity = 10;
    public int maxCapacity = 20; // Maximum items this box can hold
    
    // Physics references
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
        if (isHeld) return $"Hold Click to Stock / Right Click to Retrieve ({currentQuantity}/{maxCapacity})";
        return $"Press E to pick up {itemData.itemName} Box";
    }

    // ... OnInteract and OnAltInteract stay the same ...
    public void OnInteract(PlayerInteraction player)
    {
        if (!isHeld)
        {
            isHeld = true;
            rb.isKinematic = true; 
            col.enabled = false; 
            player.AttachToHand(this.gameObject);
        }
    }

    public void OnAltInteract(PlayerInteraction player)
    {
        if (isHeld)
        {
            isHeld = false;
            player.ReleaseFromHand();
            col.enabled = true;
            rb.isKinematic = false;
            rb.AddForce(player.playerCamera.transform.forward * 400f);
        }
    }

    // --- LOGIC FOR SHELF ---

    // Taking item OUT of box (Stocking Shelf)
    public bool TryTakeItem()
    {
        if (currentQuantity > 0)
        {
            currentQuantity--;
            return true;
        }
        return false;
    }

    // NEW: Putting item INTO box (Retrieving from Shelf)
    public bool TryAddItem()
    {
        if (currentQuantity < maxCapacity)
        {
            currentQuantity++;
            return true;
        }
        // Box is full
        return false;
    }
}