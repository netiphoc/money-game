using UnityEngine;
using Utilities;

/// <summary>
/// A static instance is similar to a singleton, but instead of destroying any new
/// instances, it overrides the current instance. This is handy for resetting the state
/// and saves you doing it manually.
/// </summary>
public abstract class Singleton<T> : BaseMonoBehaviour where T : Component
{
    private static T _instance;

    // Helper to check if application is quitting to prevent creating leaks
    private static bool _isQuitting = false;

    public static T Instance
    {
        get
        {
            if (_isQuitting)
            {
                Debug.LogWarning($"[Singleton] Instance '{typeof(T)}' already destroyed on application quit. Won't create again - returning null.");
                return null;
            }

            if (_instance == null)
            {
                // 1. Try to find an existing instance in the scene
                _instance = FindObjectOfType<T>(); // Unity 2023+ use FindFirstObjectByType<T>()

                // 2. If not found, create a new GameObject and add the component
                if (_instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(T).Name + "_AutoCreated";
                    _instance = obj.AddComponent<T>();
                    Debug.Log($"[Singleton] Created new instance of {typeof(T)}");
                }
            }
            return _instance;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        
        if (_instance == null)
        {
            _instance = this as T;
            
            // Optional: Uncomment if you want your Singletons to survive scene loads by default
            // DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Debug.LogWarning($"[Singleton] Duplicate instance of {typeof(T)} detected. Destroying duplicate.");
            Destroy(gameObject);
        }
    }

    // Prevents "Ghost" objects from being created when the game stops
    protected virtual void OnApplicationQuit()
    {
        _isQuitting = true;
    }
}