using UnityEngine;
using UnityEngine.InputSystem; // REQUIRED namespace
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Input Actions")]
    [Tooltip("Drag the 'Interact' action from your Input Asset here")]
    public InputActionProperty interactInput;      // Key: E
    [Tooltip("Drag the 'PrimaryAction' (Left Click) here")]
    public InputActionProperty primaryInput;       // Mouse: Left Click
    [Tooltip("Drag the 'SecondaryAction' (Right Click) here")]
    public InputActionProperty secondaryInput;     // Mouse: Right Click
    [Tooltip("Drag the 'Throw' action here")]
    public InputActionProperty throwInput;         // Key: G

    [Header("Settings")]
    public float interactDistance = 3f;
    public LayerMask interactLayer;

    [Header("References")]
    public Camera playerCamera;
    public Transform holdPoint;
    public TextMeshProUGUI promptText;

    [Header("State")]
    public GameObject currentHeldObject;

    // We need to enable inputs when this script turns on
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
        if (lookTarget != null)
        {
            promptText.text = lookTarget.GetInteractionPrompt();
            promptText.gameObject.SetActive(true);
            return;
        }

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
        promptText.gameObject.SetActive(false);
    }

    private void HandleInput(IInteractable lookTarget)
    {
        // --- SCENARIO A: HOLDING AN OBJECT ---
        if (currentHeldObject != null)
        {
            // 1. Stocking / Context Interaction (Passthrough)
            if (lookTarget != null)
            {
                // IsPressed() returns true every frame the button is held
                if (primaryInput.action.IsPressed()) 
                {
                    lookTarget.OnInteract(this);
                }
                else if (secondaryInput.action.IsPressed())
                {
                    lookTarget.OnAltInteract(this);
                }
            }

            // 2. Throwing Logic
            if (throwInput.action.WasPerformedThisFrame())
            {
                IInteractable heldLogic = currentHeldObject.GetComponent<IInteractable>();
                heldLogic?.OnAltInteract(this);
            }
        }
        // --- SCENARIO B: HANDS EMPTY ---
        else 
        {
            // Standard Pickup (Triggered on press, not hold)
            if (lookTarget != null)
            {
                // We check both E (Interact) and Left Click (Primary) for pickup convenience
                if (interactInput.action.WasPerformedThisFrame() || primaryInput.action.WasPerformedThisFrame())
                {
                    lookTarget.OnInteract(this);
                }
            }
        }
    }

    // Helpers
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
}