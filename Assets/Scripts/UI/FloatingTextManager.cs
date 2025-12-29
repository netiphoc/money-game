using UnityEngine;

public class FloatingTextManager : MonoBehaviour
{
    public static FloatingTextManager Instance;

    [Header("References")]
    public GameObject textPrefab; 
    public Canvas parentCanvas;
    
    [Header("Fixed Position Settings")]
    public RectTransform fixedSpawnPoint; // Drag an empty UI object here (e.g. Top Center)
    public float randomSpread = 50f;      // Jitter so texts don't stack perfectly

    [Header("World Settings")]
    public Camera mainCamera;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        if (mainCamera == null) mainCamera = Camera.main;
    }

    // --- OPTION A: Show at fixed screen location (Top of Screen) ---
    public void ShowFixedText(string text, Color color)
    {
        if (fixedSpawnPoint == null)
        {
            Debug.LogError("Please assign 'Fixed Spawn Point' in Inspector!");
            return;
        }

        // 1. Create Text
        GameObject newTextObj = Instantiate(textPrefab, parentCanvas.transform);
        
        // 2. Set Position to the Fixed Point + Random Offset
        RectTransform rt = newTextObj.GetComponent<RectTransform>();
        
        // Calculate random jitter
        Vector2 jitter = new Vector2(
            Random.Range(-randomSpread, randomSpread), 
            Random.Range(-randomSpread / 2, randomSpread / 2)
        );

        // Copy position from the spawn point anchor
        rt.position = fixedSpawnPoint.position + (Vector3)jitter;

        // 3. Initialize
        UIFloatingText floatScript = newTextObj.GetComponent<UIFloatingText>();
        if (floatScript != null)
        {
            floatScript.Setup(text, color);
        }
    }

    // --- OPTION B: Show above a 3D Object (Original) ---
    public void ShowWorldText(Vector3 worldPosition, string text, Color color)
    {
        Vector2 screenPos = mainCamera.WorldToScreenPoint(worldPosition + Vector3.up * 2f);
        
        GameObject newTextObj = Instantiate(textPrefab, parentCanvas.transform);
        newTextObj.transform.position = screenPos;

        UIFloatingText floatScript = newTextObj.GetComponent<UIFloatingText>();
        if (floatScript != null) floatScript.Setup(text, color);
    }
}