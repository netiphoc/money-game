using UnityEngine;
using TMPro;
using UnityEngine.InputSystem; // Make sure to import TextMeshPro for the UI

public class PlayerInteraction : MonoBehaviour
{
    [Header("Settings")]
    public float interactDistance = 3f;
    public LayerMask interactLayer; // Set this to "Default" or a custom "Interactable" layer

    [Header("References")]
    public Camera playerCamera;
    public Transform holdPoint; // An empty GameObject child of Camera where items sit when held
    public TextMeshProUGUI promptText; // UI Text on screen

    [Header("State")]
    public GameObject currentHeldObject; // What are we holding right now?

    private void Update()
    {
        HandleRaycast();
        HandleInput();
    }

    private void HandleRaycast()
    {
        // 1. Create a ray from center of camera
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        // 2. Shoot the ray
        if (Physics.Raycast(ray, out hit, interactDistance, interactLayer))
        {
            // 3. Try to get the Interface component from the object we hit
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                // We hit something interactive! Show the prompt.
                promptText.text = interactable.GetInteractionPrompt();
                promptText.gameObject.SetActive(true);
                
                // (Optional) Highlight effect logic would go here
                return;
            }
        }

        // If we hit nothing, hide the text
        promptText.gameObject.SetActive(false);
    }

    private void HandleInput()
    {
        // If we are holding an object, we bypass the raycast and talk directly to the held object
        if (currentHeldObject != null)
        {
             // Get the interactable logic from the thing we are holding
             IInteractable heldLogic = currentHeldObject.GetComponent<IInteractable>();
             
             // Example: Throw logic using 'G'
             if (Keyboard.current.gKey.wasPressedThisFrame)
             {
                 // You could add a specific "OnThrow" method to interface, 
                 // or handle it in the object's update.
                 // For now, we use AltInteract for throwing/dropping mechanics
                 heldLogic.OnAltInteract(this);
             }
             return;
        }

        // Standard Interaction (When not holding anything)
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactDistance, interactLayer))
            {
                IInteractable interactable = hit.collider.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    interactable.OnInteract(this);
                }
            }
        }
    }

    // Helper method to attach objects to the hand
    public void AttachToHand(GameObject obj)
    {
        currentHeldObject = obj;
        obj.transform.SetParent(holdPoint);
        obj.transform.localPosition = Vector3.zero; // Snap to hand
        obj.transform.localRotation = Quaternion.identity;
    }

    // Helper method to release objects
    public void ReleaseFromHand()
    {
        if(currentHeldObject != null)
        {
            currentHeldObject.transform.SetParent(null);
            currentHeldObject = null;
        }
    }
}