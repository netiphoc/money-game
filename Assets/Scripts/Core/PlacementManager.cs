using UnityEngine;
using UnityEngine.InputSystem;

public class PlacementManager : MonoBehaviour
{
    public static PlacementManager Instance;

    [Header("Test")] [SerializeField] private PlaceableDataSO itemToBuy;
    
    [Header("Input Actions (New Input System)")]
    public InputActionProperty rotateInput;  // Value: Mouse Scroll or Q/E
    public InputActionProperty confirmInput; // Button: Left Click
    public InputActionProperty cancelInput;  // Button: G
    public InputActionProperty boxUpInput;   // Button: Right Click

    [Header("Settings")]
    public LayerMask placementLayer; // Set to "Floor"
    public LayerMask obstacleLayer;  // Set to "Obstacle" (Walls, other furniture)
    public Material validMat;        // Transparent Green
    public Material invalidMat;      // Transparent Red
    public GameObject itemBoxPrefab; // The generic cardboard box prefab
    public float rotationSpeed = 10f;

    [Header("State (Read Only)")]
    public bool IsPlacing;

    // Internal State
    private GameObject currentGhost;     // The visual object we are currently moving
    private PlaceableDataSO currentData; // Data for the current object
    private ItemBox sourceBox;           // If unboxing, this is the box we are holding
    
    // Moving Existing Object State
    private bool isEditingExisting = false;
    private GameObject originalObjectRef; // Reference to the real object being moved
    private Vector3 originalPos;
    private Quaternion originalRot;
    private Material[] originalMaterials;
    private Collider[] originalColliders;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        rotateInput.action.Enable();
        confirmInput.action.Enable();
        cancelInput.action.Enable();
        boxUpInput.action.Enable();
    }

    private void OnDisable()
    {
        rotateInput.action.Disable();
        confirmInput.action.Disable();
        cancelInput.action.Disable();
        boxUpInput.action.Disable();
    }

    private void Update()
    {
#if  UNITY_EDITOR
        if (Keyboard.current.xKey.wasPressedThisFrame)
        {
            StartPlacement(itemToBuy);
        }
#endif
        if (!IsPlacing || currentGhost == null) return;

        HandlePositioning();
        HandleRotation();
        HandleInput();
    }

    // ==================================================================================
    // 1. ENTRY POINTS (Start Placement)
    // ==================================================================================

    // Mode A: Buying from Shop
    public void StartPlacement(PlaceableDataSO data)
    {
        StopPlacement(); // Cleanup previous state

        isEditingExisting = false;
        sourceBox = null;
        currentData = data;

        SpawnGhost(data.linkedItemData.itemPrefab);
        IsPlacing = true;
    }

    // Mode B: Unboxing from Hand (Called by ItemBox)
    public void StartPlacementFromBox(PlaceableDataSO data, ItemBox box)
    {
        StopPlacement();

        isEditingExisting = false;
        sourceBox = box;
        currentData = data;

        SpawnGhost(data.linkedItemData.itemPrefab);
        IsPlacing = true;
    }

    // Mode C: Moving Existing Furniture (Called by MoveableFurniture)
    public void StartMovingExistingObject(GameObject obj, PlaceableDataSO data)
    {
        StopPlacement();

        isEditingExisting = true;
        sourceBox = null;
        currentData = data;
        originalObjectRef = obj;

        // Store Original State (For Reset/Cancel)
        originalPos = obj.transform.position;
        originalRot = obj.transform.rotation;

        // Store Visuals & Colliders to restore later
        Renderer[] rends = obj.GetComponentsInChildren<Renderer>();
        originalMaterials = new Material[rends.Length];
        for (int i = 0; i < rends.Length; i++) originalMaterials[i] = rends[i].material;

        originalColliders = obj.GetComponentsInChildren<Collider>();

        // For existing objects, the "Ghost" IS the object itself
        currentGhost = obj; 
        
        // Disable physics/colliders while moving
        foreach (var col in originalColliders) col.enabled = false;
        if(currentGhost.TryGetComponent(out Rigidbody rb)) rb.isKinematic = true;

        IsPlacing = true;
    }

    // ==================================================================================
    // 2. CORE LOGIC
    // ==================================================================================

    private void HandlePositioning()
    {
        // New Input System: Get mouse position directly
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 20f, placementLayer))
        {
            // Optional: Add Grid Snapping here if desired
            currentGhost.transform.position = hit.point;
        }
    }

    private void HandleRotation()
    {
        float scroll = rotateInput.action.ReadValue<float>();
        
        // Check threshold to avoid jitter
        if (Mathf.Abs(scroll) > 0.01f)
        {
            float direction = scroll > 0 ? 1 : -1;
            float step = 45f; // Snap to 45 degrees
            
            // Rotate around Up Axis
            currentGhost.transform.Rotate(Vector3.up * direction * step * rotationSpeed * Time.deltaTime);
        }
    }

    private void HandleInput()
    {
        bool isValid = CheckValidity();
        UpdateGhostMaterials(isValid);

        // 1. CONFIRM (Primary Action / Left Click)
        if (confirmInput.action.WasPerformedThisFrame())
        {
            if (isValid) TryFinalizePlacement();
            else { /* Play Error Sound */ }
        }

        // 2. CANCEL (Throw Input / G)
        if (cancelInput.action.WasPerformedThisFrame())
        {
            CancelPlacement();
        }

        // 3. BOX UP (Secondary Action / Right Click)
        // Only allow boxing up if we are editing an EXISTING object.
        // If we are just placing a new one from a box, Right Click shouldn't do anything (or could cancel).
        if (boxUpInput.action.WasPerformedThisFrame())
        {
            if (isEditingExisting)
            {
                PerformBoxUp();
            }
            else
            {
                // Optional: If holding a new box, Right Click could just Cancel
                CancelPlacement();
            }
        }
    }

    // ==================================================================================
    // 3. FINALIZATION LOGIC
    // ==================================================================================

    private void TryFinalizePlacement()
    {
        // Scenario 1: Unboxing (Standard Flow)
        if (sourceBox != null)
        {
            // We don't check money here. You already paid for the box.
            if (sourceBox.TryTakeItem())
            {
                PlaceObject();
            }
            else
            {
                CancelPlacement(); // Box empty
            }
        }
        // Scenario 2: Moving Existing (Renovating)
        else if (isEditingExisting)
        {
            RestoreOriginalObjectVisuals();
            IsPlacing = false;
            CleanupVariables();
        }
        
        // We removed the "Scenario 3: Buying New" because that loop no longer exists.
    }

    private void PlaceObject()
    {
        if (!isEditingExisting)
        {
            // Spawn the real object at the ghost's location
            Instantiate(currentData.linkedItemData.itemPrefab, currentGhost.transform.position, currentGhost.transform.rotation);
            Destroy(currentGhost); // Destroy the ghost
        }

        IsPlacing = false;
        CleanupVariables();
    }

    // ==================================================================================
    // 4. CANCEL & RESET LOGIC
    // ==================================================================================

    public void StopPlacement()
    {
        CancelPlacement();
    }

    private void CancelPlacement()
    {
        if (!IsPlacing) return;

        // If we were moving an existing object, put it back
        if (isEditingExisting && originalObjectRef != null)
        {
            // Reset to original position
            originalObjectRef.transform.position = originalPos;
            originalObjectRef.transform.rotation = originalRot;
            
            RestoreOriginalObjectVisuals();
        }
        // If we were placing a NEW object (Buy or Unbox), just delete the ghost
        else if (currentGhost != null)
        {
            Destroy(currentGhost);
        }

        IsPlacing = false;
        CleanupVariables();
    }

    private void PerformBoxUp()
    {
        // 1. Capture the position (just for safety)
        Vector3 spawnPos = currentGhost.transform.position;

        // 2. Destroy the furniture object (the one we were moving)
        Destroy(currentGhost);

        // 3. Spawn the Cardboard Box
        GameObject newBox = Instantiate(itemBoxPrefab, spawnPos, Quaternion.identity);
        ItemBox boxScript = newBox.GetComponent<ItemBox>();

        // 4. Configure Box Data (What's inside?)
        if (boxScript != null && currentData.linkedItemData != null)
        {
            boxScript.itemData = currentData.linkedItemData;
            boxScript.currentQuantity = 1;
        }

        // 5. WARP TO HAND LOGIC
        // Find the player and force them to grab the box immediately
        PlayerInteraction player = FindObjectOfType<PlayerInteraction>();
        if (player != null && boxScript != null)
        {
            // We call OnInteract directly, which handles:
            // - Setting isHeld = true
            // - Disabling Physics/Colliders
            // - Snapping to the Hand position
            boxScript.OnInteract(player);
        }

        // 6. Exit Placement Mode
        IsPlacing = false;
        CleanupVariables();
    }

    // ==================================================================================
    // 5. HELPER METHODS
    // ==================================================================================

    private void SpawnGhost(GameObject prefab)
    {
        currentGhost = Instantiate(prefab);
        
        // Remove physics/scripts from ghost so it doesn't function while placing
        if(currentGhost.TryGetComponent(out Rigidbody rb)) Destroy(rb);
        foreach (var col in currentGhost.GetComponentsInChildren<Collider>()) Destroy(col);
    }

    private void RestoreOriginalObjectVisuals()
    {
        if (originalObjectRef == null) return;

        // Restore Materials
        Renderer[] rends = originalObjectRef.GetComponentsInChildren<Renderer>();
        for (int i = 0; i < rends.Length; i++)
        {
            if (i < originalMaterials.Length) rends[i].material = originalMaterials[i];
        }

        // Restore Colliders
        foreach (var col in originalColliders) 
            if(col != null) col.enabled = true;
    }

    private void CleanupVariables()
    {
        currentGhost = null;
        currentData = null;
        sourceBox = null;
        isEditingExisting = false;
        originalObjectRef = null;
        originalMaterials = null;
        originalColliders = null;
    }

    private bool CheckValidity()
    {
        if (currentData == null || currentGhost == null) return false;

        Vector3 center = currentGhost.transform.position + new Vector3(0, currentData.boxSize.y / 2, 0);
        
        // Check for overlaps with Obstacle Layer
        Collider[] hits = Physics.OverlapBox(center, currentData.boxSize / 2, currentGhost.transform.rotation, obstacleLayer);
        
        foreach (var hit in hits)
        {
            // If moving existing, ignore self
            if (isEditingExisting && hit.gameObject == originalObjectRef) continue;
            // Ignore child colliders
            if (hit.transform.IsChildOf(currentGhost.transform)) continue;
            
            return false; // Collision detected
        }

        return true;
    }

    private void UpdateGhostMaterials(bool isValid)
    {
        Material mat = isValid ? validMat : invalidMat;
        Renderer[] rends = currentGhost.GetComponentsInChildren<Renderer>();
        foreach (var r in rends) r.material = mat;
    }

    private void OnDrawGizmos()
    {
        if (IsPlacing && currentGhost != null && currentData != null)
        {
            Gizmos.color = CheckValidity() ? Color.green : Color.red;
            Gizmos.matrix = Matrix4x4.TRS(currentGhost.transform.position, currentGhost.transform.rotation, Vector3.one);
            Gizmos.DrawWireCube(new Vector3(0, currentData.boxSize.y / 2, 0), currentData.boxSize);
        }
    }
}