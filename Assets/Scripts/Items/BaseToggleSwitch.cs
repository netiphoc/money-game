using Systems;
using UI;
using UnityEngine;
using UnityEngine.Events;

public abstract class BaseToggleSwitch : BaseInteractable
{
    [Header("Settings")]
    public string objectName = "Breaker";
    public bool isOn = false;

    [Header("Events")]
    public UnityEvent OnToggleOn;
    public UnityEvent OnToggleOff;

    protected override void InitInteractionPrompts()
    {
        AddInteractionPrompt(new []
        {
            new InteractionPromptData
            {
                Icon = KeyIcon.MOUSE_LEFT_CLICK,
                Prompt = "Turn on"
            },
        });
        
        AddInteractionPrompt(new []
        {
            new InteractionPromptData
            {
                Icon = KeyIcon.MOUSE_LEFT_CLICK,
                Prompt = "Turn off"
            },
        });
        
        AddInteractionPrompt(new []
        {
            new InteractionPromptData
            {
                Icon = KeyIcon.ALERT,
                Prompt = GetDenyPrompt()
            },
        });
    }

    public override InteractionPromptData[] GetInteractionPrompts()
    {
        if (!ToggleCondition())
        {
            return GetInteractionPromptByIndex(2);
        }

        return isOn ? GetInteractionPromptByIndex(1) : GetInteractionPromptByIndex(0);
    }
    
    public override BaseUI GetUI()
    {
        return default;
    }

    protected virtual string GetDenyPrompt()
    {
        return "Deny";
    }
    
    public override void OnInteract(PlayerInteraction player)
    {
        if(!ToggleCondition()) return;
        
        if (player.WasPrimaryPressed())
        {
            Toggle();
        }
    }

    public override void OnAltInteract(PlayerInteraction player) { }

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