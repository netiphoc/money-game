using UnityEngine;
using System;
using Utilities;

public class GameTimeManager : BaseMonoBehaviour
{
    protected override void Awake()
    {
        base.Awake();
        InitializeTimer();
    }

    [Header("Configuration")]
    [Tooltip("The time loop lasts this many real-time minutes.")]
    [SerializeField] private float realTimeDurationMinutes = 15f; 
    
    [Tooltip("Start hour in 24h format (10 = 10AM)")]
    [SerializeField] private float startHour = 10f;
    
    [Tooltip("End hour in 24h format (22 = 10PM)")]
    [SerializeField] private float endHour = 22f;

    [Header("Debug Info")]
    [SerializeField] private bool isRunning = false;
    [SerializeField] private string currentTimeString;

    // --- EVENTS ---
    
    /// <summary>
    /// Triggered every 1 REAL-TIME second. 
    /// Use for: Cooldowns, resource regeneration, timers unrelated to game clock.
    /// </summary>
    public event Action OnRealSecondTick;

    /// <summary>
    /// Triggered every 1 IN-GAME Minute.
    /// Use for: Updating UI Clock, NPC Schedules.
    /// </summary>
    public event Action<string> OnGameMinuteTick; // Sends the formatted time string

    /// <summary>
    /// Triggered when the clock hits 10:00 PM.
    /// </summary>
    public event Action OnTimeEnded;

    // Internal Calculations
    private float _totalSecondsNeeded;
    private float _currentRealTimeAccumulator = 0f;
    private float _oneSecondTimer = 0f;
    private int _lastGameMinute = -1; // To track when a minute changes

    // Properties
    public float CurrentGameHour { get; private set; }

    private void InitializeTimer()
    {
        // Calculate total duration in seconds (15 * 60 = 900)
        _totalSecondsNeeded = realTimeDurationMinutes * 60f;
        
        // Reset to start
        CurrentGameHour = startHour;
        _currentRealTimeAccumulator = 0f;
        
        // Calculate initial minute for tracking
        _lastGameMinute = Mathf.FloorToInt(CurrentGameHour * 60);
        
        UpdateDebugString();
    }


    protected override void Update()
    {
        base.Update();
        
        if (!isRunning) return;

        // 1. Advance Real Time
        float dt = Time.deltaTime;
        _currentRealTimeAccumulator += dt;

        // 2. Check End Condition
        if (_currentRealTimeAccumulator >= _totalSecondsNeeded)
        {
            ForceEndTime();
            return;
        }

        // 3. Calculate Game Time (Lerp)
        float progressPercent = _currentRealTimeAccumulator / _totalSecondsNeeded;
        CurrentGameHour = Mathf.Lerp(startHour, endHour, progressPercent);

        // 4. Handle "Game Minute Tick" (The Clock Tick)
        CheckGameMinuteTick();

        // 5. Handle "Real Second Tick"
        CheckRealSecondTick(dt);

        // Debug visualization
        UpdateDebugString();
    }

    private void CheckGameMinuteTick()
    {
        // Convert current hour float to total minutes (e.g., 10.5h = 630 mins)
        int currentTotalMinutes = Mathf.FloorToInt(CurrentGameHour * 60);

        // If the minute integer has changed since the last frame
        if (currentTotalMinutes > _lastGameMinute)
        {
            _lastGameMinute = currentTotalMinutes;
            
            // Fire event with the formatted string (e.g., "10:01 AM")
            OnGameMinuteTick?.Invoke(GetFormattedTime());
        }
    }

    private void CheckRealSecondTick(float dt)
    {
        _oneSecondTimer += dt;
        if (_oneSecondTimer >= 1.0f)
        {
            _oneSecondTimer -= 1.0f; 
            OnRealSecondTick?.Invoke();
        }
    }

    private void ForceEndTime()
    {
        _currentRealTimeAccumulator = _totalSecondsNeeded;
        CurrentGameHour = endHour;
        isRunning = false;
        
        // Ensure the display shows exactly the end time
        UpdateDebugString();
        OnGameMinuteTick?.Invoke(GetFormattedTime());
        
        OnTimeEnded?.Invoke();
        Debug.Log("Day Ended.");
    }

    private void UpdateDebugString()
    {
        currentTimeString = GetFormattedTime();
    }

    // --- PUBLIC API ---

    public void StartClock()
    {
        if (_currentRealTimeAccumulator >= _totalSecondsNeeded)
        {
            Debug.LogWarning("Time already finished. Call ResetClock() first?");
            return;
        }
        isRunning = true;
    }

    public void StopClock()
    {
        isRunning = false;
    }

    public void ResetClock()
    {
        isRunning = false;
        InitializeTimer();
        // Fire tick to update UI to start time
        OnGameMinuteTick?.Invoke(GetFormattedTime());
    }

    public bool IsStartOfDay()
    {
        // We use >= just to be safe with floating point numbers
        return CurrentGameHour <= startHour;
    }
    public bool IsEndOfDay()
    {
        // We use >= just to be safe with floating point numbers
        return CurrentGameHour >= endHour;
    }
    
    public string GetFormattedTime()
    {
        int totalMinutes = Mathf.FloorToInt(CurrentGameHour * 60);
        int hours = totalMinutes / 60;
        int minutes = totalMinutes % 60;

        string period = "AM";
        
        // Simple 12h formatting logic
        if (hours >= 12)
        {
            period = "PM";
            if (hours > 12) hours -= 12;
        }
        // Handle midnight (00:00) case if needed, though range is 10am-10pm
        if (hours == 0) hours = 12;

        return string.Format("{0:00}:{1:00} {2}", hours, minutes, period);
    }
}