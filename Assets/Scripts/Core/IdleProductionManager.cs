using UnityEngine;

public class IdleProductionManager : MonoBehaviour
{
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
        foreach (var room in GameManager.Instance.GymRooms)
        {
            // 1. Check if room has a worker
            if (room.assignedBoxer != null)
            {
                BoxerData stats = room.assignedBoxer.stats;
                
                // 1. Decay Hunger/Sleep
                stats.TickSurvival();
                
                // 2. Get Survival Penalty (The new logic)
                float survivalMult = stats.GetEfficiencyMultiplier();

                // 2. Apply the Room's Calculated Rates
                float strGain = room.totalStrRate;
                float agiGain = room.totalAgiRate;
                float staGain = room.totalStaRate;
                float sleepGain = room.totalSleepRate;
                float hungerGain = room.totalHungerRate;

                // 3. Apply Boxer's Personal Multiplier (Pro vs Rookie)
                float mult = stats.statMultiplier * survivalMult;
                
                stats.unrealizedStrength += strGain * mult;
                stats.unrealizedAgility += agiGain * mult;
                stats.unrealizedStamina += staGain * mult;
                stats.unrealizedSleep += sleepGain;
                stats.unrealizedHunger += hungerGain;
            }
        }
    }
}