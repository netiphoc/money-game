using UnityEngine;

public class ItemBox : MonoBehaviour, IInteractable
{
    [Header("Box Data")]
    public ItemDataSO itemData;
    public int currentQuantity = 10;
    public int maxCapacity = 20;

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
        if (isHeld) return $"Hold LMB Stock / RMB Retrieve / G Throw";
        return $"Press E to pick up {itemData.itemName} Box";
    }

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

    // Used for logic when looking at OTHER things? No, this is empty now.
    public void OnAltInteract(PlayerInteraction player) { }

    // --- NEW SPECIFIC THROW METHOD ---
    public void Throw(PlayerInteraction player)
    {
        if (isHeld)
        {
            isHeld = false;
            player.ReleaseFromHand();
            
            col.enabled = true;
            rb.isKinematic = false;
            
            // Add force forward
            rb.AddForce(player.playerCamera.transform.forward * 600f); 
            // Optional: Add some torque spin
            rb.AddTorque(Random.insideUnitSphere * 100f);
        }
    }

    // --- SHELF LOGIC HELPER METHODS ---
    public bool TryTakeItem()
    {
        if (currentQuantity > 0)
        {
            currentQuantity--;
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