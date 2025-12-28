using UnityEngine;

public class IdleProductionManager : MonoBehaviour
{
    public static IdleProductionManager Instance;

    [SerializeField] private GymRoom[] allRooms; // Cache for performance

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        GameManager.Instance.GameTimeManager.OnGameMinuteTick += OnGameMinuteTick;
    }

    private void OnDestroy()
    {
        GameManager.Instance.GameTimeManager.OnGameMinuteTick -= OnGameMinuteTick;
    }

    private void OnGameMinuteTick(string format)
    {
        ProduceStats();
    }

    private void ProduceStats()
    {
        // Loop through every room in the gym
        foreach (var room in allRooms)
        {
            // 1. Check if room has a worker
            if (room.assignedBoxer != null)
            {
                BoxerData stats = room.assignedBoxer.stats;

                // 2. Apply the Room's Calculated Rates
                float strGain = room.totalStrRate;
                float agiGain = room.totalAgiRate;
                float staGain = room.totalStaRate;

                // 3. Apply Boxer's Personal Multiplier (Pro vs Rookie)
                float mult = stats.statMultiplier;
                
                stats.strength += strGain * mult;
                stats.agility += agiGain * mult;
                stats.stamina += staGain * mult;

                // 4. Update Total Power
                stats.UpdateTotal();

                // Optional: Spawn "+1" floating text here?
                // Inside ProduceStats loop, after updating stats:
                if (showFloatingText && (strGain > 0 || agiGain > 0))
                {
                    // Only spawn text if looking at the room? (Optimization for 16 rooms)
                    // For now, spawn above boxer head
                    SpawnPopup(room.assignedBoxer.transform.position, $"+{strGain+agiGain+staGain}", Color.green);
                }

            }
        }
    }

    // Call this if you unlock a new room so it gets added to the loop
    public void RefreshRoomList()
    {
        allRooms = FindObjectsOfType<GymRoom>();
    }
    
    [Header("Visuals")]
    public FloatingText floatingTextPrefab;
    public bool showFloatingText = true;
    
    private void SpawnPopup(Vector3 position, string text, Color color)
    {
        Vector3 spawnPos = position + Vector3.up * 2f;
        FloatingText popup = Instantiate(floatingTextPrefab, spawnPos, Quaternion.identity);
        popup.Setup(text, color);
    }
}