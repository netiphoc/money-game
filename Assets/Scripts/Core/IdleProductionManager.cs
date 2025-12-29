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
                
                stats.unrealizedStrength += strGain * mult;
                stats.unrealizedAgility += agiGain * mult;
                stats.unrealizedStamina += staGain * mult;
            }
        }
    }

    // Call this if you unlock a new room so it gets added to the loop
    public void RefreshRoomList()
    {
        allRooms = FindObjectsOfType<GymRoom>();
    }
    
}