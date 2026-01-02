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
                if(room.assignedBoxer.currentState == BoxerController.AIState.Fight) continue;
                
                BoxerData stats = room.assignedBoxer.stats;
                
                // 1. Decay Hunger/Sleep
                stats.TickSurvival();
                
                float hungerGain = room.totalHungerRate;
                float sleepGain = room.totalSleepRate;
                stats.unrealizedHunger += hungerGain;
                stats.unrealizedSleep += sleepGain;
                stats.ApplyUnrealizeSurvivalStats();
                
                if(room.assignedBoxer.currentState != BoxerController.AIState.TrainingVisual) continue;
                
                // 2. Get Survival Penalty (The new logic)
                float survivalMult = stats.GetEfficiencyMultiplier();

                // 2. Apply the Room's Calculated Rates
                float strGain = room.totalStrRate;
                float agiGain = room.totalAgiRate;
                float staGain = room.totalStaRate;

                // 3. Apply Boxer's Personal Multiplier (Pro vs Rookie)
                float mult = survivalMult;
                
                stats.unrealizedStrength += strGain * mult;
                stats.unrealizedAgility += agiGain * mult;
                stats.unrealizedStamina += staGain * mult;
            }
        }
    }
}