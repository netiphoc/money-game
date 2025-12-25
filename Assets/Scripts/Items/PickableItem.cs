using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PickableItem : MonoBehaviour, IInteractable
{
    private Rigidbody rb;
    private bool isHeld = false;

    [Header("Settings")]
    public string itemName = "Box";
    public float throwForce = 500f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public string GetInteractionPrompt()
    {
        if (isHeld) return "Press G to Throw";
        return $"Press E to pick up {itemName}";
    }

    public void OnInteract(PlayerInteraction player)
    {
        // Pick Up Logic
        if (!isHeld)
        {
            isHeld = true;
            rb.isKinematic = true; // Disable physics so it doesn't fall out of hand
            rb.useGravity = false;
            
            // Disable collision with player so you don't push yourself
            GetComponent<Collider>().enabled = false; 

            player.AttachToHand(this.gameObject);
        }
    }

    public void OnAltInteract(PlayerInteraction player)
    {
        // Throw / Drop Logic
        if (isHeld)
        {
            isHeld = false;
            player.ReleaseFromHand();

            // Re-enable physics
            GetComponent<Collider>().enabled = true;
            rb.isKinematic = false;
            rb.useGravity = true;

            // Add Throw Force
            rb.AddForce(player.playerCamera.transform.forward * throwForce);
        }
    }
}