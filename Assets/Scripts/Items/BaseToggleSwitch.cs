using UI;
using UnityEngine;
using UnityEngine.Events;

public abstract class BaseToggleSwitch : MonoBehaviour, IInteractable
{
    [Header("Settings")]
    public string objectName = "Breaker";
    public bool isOn = false;

    [Header("Events")]
    public UnityEvent OnToggleOn;
    public UnityEvent OnToggleOff;

    public string GetInteractionPrompt()
    {
        if (!ToggleCondition())
        {
            return GetDenyPrompt();
        }
        
        string status = isOn ? "OFF" : "ON";
        return $"Press Left Click to Turn {status} {objectName}";
    }

    public BaseUI GetUI()
    {
        return default;
    }

    protected virtual string GetDenyPrompt()
    {
        return "Deny";
    }
    
    public void OnInteract(PlayerInteraction player)
    {
        if(!ToggleCondition()) return;
        
        if (player.WasPrimaryPressed())
        {
            Toggle();
        }
    }

    public void OnAltInteract(PlayerInteraction player) { }

    protected virtual void Toggle()
    {
        isOn = !isOn;

        // 1. Trigger Gameplay Events
        if (isOn) OnToggleOn?.Invoke();
        else OnToggleOff?.Invoke();
    }

    protected virtual bool ToggleCondition()
    {
        return true;
    }
}