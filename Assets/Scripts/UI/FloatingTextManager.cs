using System;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public enum TextSpawnPointType
{
    Exp, Alert, Money
}

public class FloatingTextManager : MonoBehaviour
{
    public static FloatingTextManager Instance;

    [Header("Floating Text 2D")]
    public UIFloatingText textPrefab; 
    public Canvas parentCanvas;
    [Space]
    public RectTransform fixedSpawnPoint; // Drag an empty UI object here (e.g. Top Center)
    public RectTransform moneySpawnPoint; // Drag an empty UI object here (e.g. Top Center)
    public RectTransform expSpawnPoint; // Drag an empty UI object here (e.g. Top Center)
    public float randomSpread = 50f;      // Jitter so texts don't stack perfectly
    [Space]
    public Camera mainCamera;
    
    [Header("Floating Text 3D")]
    public FloatingText floatingTextPrefab;
    
    [Header("Floating Text Collect")]
    public UIFloatingTextCollect uiFloatingTextCollectPrefab;
    public RectTransform moneyIconPointStart; 
    public RectTransform moneyIconPointEnd; 
    
    [Header("Floating Icon Collect")]
    public UIFloatingIconCollect uiFloatingIconCollectPrefab;

    private PoolingUI<FloatingText> _poolingWorldText;
    private PoolingUI<UIFloatingText> _poolingUIText;
    private PoolingUI<UIFloatingTextCollect> _poolingUITextCollect;
    private PoolingUI<UIFloatingIconCollect> _poolingUIIconCollect;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        if (mainCamera == null) mainCamera = Camera.main;
        
        _poolingWorldText = new PoolingUI<FloatingText>(floatingTextPrefab);
        _poolingUIText = new PoolingUI<UIFloatingText>(textPrefab);
        _poolingUITextCollect = new PoolingUI<UIFloatingTextCollect>(uiFloatingTextCollectPrefab);
        _poolingUIIconCollect = new PoolingUI<UIFloatingIconCollect>(uiFloatingIconCollectPrefab);
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            int amount = Random.Range(100, 1000);
            GameManager.Instance.AddMoney(amount);
            ShowMoneyText(amount);
        }
#endif
    }

    // --- OPTION A: Show at fixed screen location (Top of Screen) ---
    public void ShowFixedText(string text, Color color, TextSpawnPointType textSpawnPointType = TextSpawnPointType.Alert)
    {
        // 1. Create Text
        UIFloatingText newTextObj = _poolingUIText.RequestRecycle(parentCanvas.transform);
        
        // 2. Set Position to the Fixed Point + Random Offset
        RectTransform rt = newTextObj.GetComponent<RectTransform>();
        
        // Calculate random jitter
        Vector2 jitter = new Vector2(
            Random.Range(-randomSpread, randomSpread), 
            Random.Range(-randomSpread / 2, randomSpread / 2)
        );

        // Copy position from the spawn point anchor
        
        Transform point;
        switch (textSpawnPointType)
        {
            case TextSpawnPointType.Exp:
                point = expSpawnPoint;
                break;
            case TextSpawnPointType.Money:
                point = moneySpawnPoint;
                break;
            case TextSpawnPointType.Alert:
            default:
                point = fixedSpawnPoint;
                break;
        }
        
        rt.position = point.position + (Vector3)jitter;

        // 3. Initialize
        if (newTextObj != null)
        {
            newTextObj.Setup(text, color);
        }
    }

    // --- OPTION B: Show above a 3D Object (Original) ---
    public void ShowWorldTextOnScreen(Vector3 worldPosition, string text, Color color)
    {
        Vector2 screenPos = mainCamera.WorldToScreenPoint(worldPosition + Vector3.up * 2f);

        UIFloatingText newTextObj = _poolingUIText.RequestRecycle(parentCanvas.transform);
        newTextObj.transform.position = screenPos;

        if (newTextObj != null) newTextObj.Setup(text, color);
    }
    
    public void ShowWorldText(Vector3 position, string text, Color color)
    {
        Vector3 spawnPos = position + Vector3.up * 2f;
        FloatingText popup = _poolingWorldText.RequestRecycle(transform);
        popup.transform.position = spawnPos;
        popup.transform.rotation = Quaternion.identity;
        popup.Setup(text, color);
    }

    /// <summary>
    /// Spawns floating text that flies from a 3D world object to the UI Money counter.
    /// </summary>
    public void ShowMoneyText(Vector3 worldPosition, int amount)
    {
        // 1. Convert World Position (Player/Loot) to Screen Position
        Vector2 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
        UIFloatingTextCollect popup = _poolingUITextCollect.RequestRecycle(transform);

        // 3. Setup
        Vector2 targetPos = moneyIconPointEnd.position; // The position of your Money Icon
        popup.Initialize($"${amount:F0}", screenPosition, targetPos);
    }
    
    /// <summary>
    /// Spawns floating text that flies from UI object to the UI Money counter.
    /// </summary>
    public void ShowMoneyText(int amount)
    {
        // 1. Convert World Position (Player/Loot) to Screen Position
        UIFloatingTextCollect popup = _poolingUITextCollect.RequestRecycle(transform);

        // 3. Setup
        Vector2 targetPos = moneyIconPointEnd.position; // The position of your Money Icon
        popup.Initialize($"${amount:F0}", moneyIconPointStart.position, targetPos);
    }
    
    /// <summary>
    /// Spawns floating icon that flies from a UI object to the UI Money counter.
    /// </summary>
    public void ShowFlyingIcon(Sprite icon, Vector2 from, Vector2 to)
    {
        UIFloatingIconCollect popup = _poolingUIIconCollect.RequestRecycle(transform);
        popup.Initialize(icon, from, to);
    }
}