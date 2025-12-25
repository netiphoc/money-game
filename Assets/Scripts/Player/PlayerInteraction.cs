using UnityEngine;
using UnityEngine.InputSystem; 
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Input Actions (New Input System)")]
    public InputActionProperty interactInput;      // Keyboard: E
    public InputActionProperty primaryInput;       // Mouse: Left Click
    public InputActionProperty secondaryInput;     // Mouse: Right Click
    public InputActionProperty throwInput;         // Keyboard: G

    [Header("Settings")]
    public float interactDistance = 3f;
    public LayerMask interactLayer;

    [Header("References")]
    public Camera playerCamera;
    public Transform holdPoint;
    public TextMeshProUGUI promptText;

    [Header("State")]
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
        IInteractable lookTarget = GetLookTarget();
        UpdateUI(lookTarget);
        HandleInput(lookTarget);
    }

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
        // 1. Priority: Look Target (e.g. Shelf, Trash, Door)
        if (lookTarget != null)
        {
            promptText.text = lookTarget.GetInteractionPrompt();
            promptText.gameObject.SetActive(true);
            return;
        }

        // 2. Priority: Held Object (e.g. "Press G to Throw")
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

        // 3. Default: Hide
        promptText.gameObject.SetActive(false);
    }

    private void HandleInput(IInteractable lookTarget)
    {
        // --- HANDS FULL ---
        if (currentHeldObject != null)
        {
            // 1. Throw Logic (G Key)
            if (throwInput.action.WasPerformedThisFrame())
            {
                // We assume the held object has an ItemBox script or similar
                ItemBox box = currentHeldObject.GetComponent<ItemBox>();
                if (box != null) box.Throw(this);
                return;
            }

            // 2. Passthrough Logic (Stocking/Retrieving while holding box)
            if (lookTarget != null)
            {
                // Primary (Hold allowed) -> Stock Shelf
                if (primaryInput.action.IsPressed()) 
                {
                    lookTarget.OnInteract(this);
                }
                // Secondary (Hold allowed) -> Retrieve from Shelf
                else if (secondaryInput.action.IsPressed())
                {
                    lookTarget.OnAltInteract(this);
                }
            }
        }
        // --- HANDS EMPTY ---
        else 
        {
            // Standard Interaction
            if (lookTarget != null)
            {
                // E Key (Single Press) OR Left Click (Single Press)
                // We use WasPerformedThisFrame so we don't spam clicks on doors/walls
                if (interactInput.action.WasPerformedThisFrame() || primaryInput.action.WasPerformedThisFrame())
                {
                    lookTarget.OnInteract(this);
                }
                // Renovation Case: If looking at a dirty wall, we might need HOLD logic
                // The TrashObject/RenovationObject checks "IsPressed" internally inside their OnInteract
                // so simply calling OnInteract here for "IsPressed" covers that too if needed.
                else if (primaryInput.action.IsPressed())
                {
                    // Only pass the continuous signal if the target specifically needs holding (like Trash)
                    // We let the target handle the logic.
                    lookTarget.OnInteract(this);
                }
            }
        }
    }

    // --- HELPER METHODS ---

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
    
    // Helper to let external scripts know if the button was JUST pressed vs HELD
    public bool IsPrimaryPressed() => primaryInput.action.IsPressed();
    public bool WasPrimaryPressed() => primaryInput.action.WasPerformedThisFrame();
}