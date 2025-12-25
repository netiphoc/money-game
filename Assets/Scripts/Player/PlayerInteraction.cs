using UnityEngine;
using UnityEngine.InputSystem; 
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Input Actions (New Input System)")]
    // Drag these from your Input Action Asset in Inspector
    public InputActionProperty interactInput;      // Key: E
    public InputActionProperty primaryInput;       // Mouse: Left Click (Used for Stocking/Placing)
    public InputActionProperty secondaryInput;     // Mouse: Right Click (Used for Retrieving/Boxing)
    public InputActionProperty throwInput;         // Key: G

    [Header("Settings")]
    public float interactDistance = 3f;
    public LayerMask interactLayer;

    [Header("References")]
    public Camera playerCamera;
    public Transform holdPoint;
    public TextMeshProUGUI promptText;

    [Header("State (Read Only)")]
    public GameObject currentHeldObject;

    private void OnEnable()
    {
        interactInput.action.Enable();
        primaryInput.action.Enable();
        secondaryInput.action.Enable();
        throwInput.action.Enable();
    }

    private void OnDisable()
    {
        interactInput.action.Disable();
        primaryInput.action.Disable();
        secondaryInput.action.Disable();
        throwInput.action.Disable();
    }

    private void Update()
    {
        // 1. BLOCKER: If Placement System is active, do nothing.
        // This prevents the player from throwing/grabbing while trying to place furniture.
        if (PlacementManager.Instance.IsPlacing) 
        {
            promptText.gameObject.SetActive(false); // Hide prompt while placing
            return;
        }

        // 2. Standard Logic
        IInteractable lookTarget = GetLookTarget();
        UpdateUI(lookTarget);
        HandleInput(lookTarget);
    }

    // ==================================================================================
    // 1. RAYCAST & UI
    // ==================================================================================

    private IInteractable GetLookTarget()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, interactDistance, interactLayer))
        {
            return hit.collider.GetComponent<IInteractable>();
        }
        return null;
    }

    private void UpdateUI(IInteractable lookTarget)
    {
        // Priority 1: Look Target (e.g., Shelf, Trash, Door)
        if (lookTarget != null)
        {
            promptText.text = lookTarget.GetInteractionPrompt();
            promptText.gameObject.SetActive(true);
            return;
        }

        // Priority 2: Held Object (e.g., "Press G to Throw")
        if (currentHeldObject != null)
        {
            IInteractable heldLogic = currentHeldObject.GetComponent<IInteractable>();
            if (heldLogic != null)
            {
                promptText.text = heldLogic.GetInteractionPrompt();
                promptText.gameObject.SetActive(true);
                return;
            }
        }

        // Default: Hide
        promptText.gameObject.SetActive(false);
    }

    // ==================================================================================
    // 2. INPUT HANDLING
    // ==================================================================================

    private void HandleInput(IInteractable lookTarget)
    {
        // --- SCENARIO A: HANDS FULL ---
        if (currentHeldObject != null)
        {
            // 1. Throw Logic (G Key)
            if (throwInput.action.WasPerformedThisFrame())
            {
                ItemBox box = currentHeldObject.GetComponent<ItemBox>();
                if (box != null) box.Throw(this);
                return;
            }

            // 2. Pass-through Logic (Context Sensitive)
            // Allows using the held item on something else (e.g. Stocking Shelf)
            // Or using the held item itself (e.g. Unboxing Furniture)
            
            // Check Interactable Target First (Shelf)
            if (lookTarget != null)
            {
                if (primaryInput.action.IsPressed()) lookTarget.OnInteract(this);
                else if (secondaryInput.action.IsPressed()) lookTarget.OnAltInteract(this);
            }
            // If looking at nothing, talk to the Held Object (Unboxing)
            else
            {
                IInteractable heldLogic = currentHeldObject.GetComponent<IInteractable>();
                if (heldLogic != null)
                {
                    if (primaryInput.action.WasPerformedThisFrame()) heldLogic.OnInteract(this);
                }
            }
        }
        // --- SCENARIO B: HANDS EMPTY ---
        else 
        {
            if (lookTarget != null)
            {
                // Primary Interaction (Pick Up / Open Door)
                // We allow both E and Click for convenience, but prevent "Hold" spam unless needed
                if (interactInput.action.WasPerformedThisFrame() || primaryInput.action.WasPerformedThisFrame())
                {
                    lookTarget.OnInteract(this);
                }
                // Continuous Interaction (Scraping Walls / Moving Furniture)
                // These scripts check 'IsPrimaryPressed()' internally, so we just pass the signal
                else if (primaryInput.action.IsPressed())
                {
                    lookTarget.OnInteract(this);
                }

                // Secondary Interaction (Auto-Retrieve Box from Shelf)
                if (secondaryInput.action.IsPressed())
                {
                    lookTarget.OnAltInteract(this);
                }
            }
        }
    }

    // ==================================================================================
    // 3. PUBLIC HELPERS
    // ==================================================================================

    public void AttachToHand(GameObject obj)
    {
        currentHeldObject = obj;
        obj.transform.SetParent(holdPoint);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
    }

    public void ReleaseFromHand()
    {
        if (currentHeldObject != null)
        {
            currentHeldObject.transform.SetParent(null);
            currentHeldObject = null;
        }
    }

    public GameObject GetHeldObject() => currentHeldObject;

    // Helper for external scripts (like MoveableFurniture) to check input state
    public bool IsPrimaryPressed() => primaryInput.action.IsPressed();
    public bool WasPrimaryPressed() => primaryInput.action.WasPerformedThisFrame();
}