using Systems;
using UnityEngine;

public class RenovatableSurface : BaseInteractable
{
    public enum SurfaceType { Wall, Floor }
    public enum State { Dirty, Clean }

    [Header("Settings")]
    public SurfaceType surfaceType;
    public State currentState = State.Dirty;
    public float scrapTime = 1.5f;

    [Header("Materials")]
    public Material dirtyMaterial; // The "Old/Peeling" look
    public Material defaultCleanMaterial; // The "Concrete/Primer" look
    public Material[] availablePatterns; // Array of paint/tile options

    // Internal State
    private float scrapTimer = 0f;
    private float lastInteractTime;
    private int currentPatternIndex = 0;
    private MeshRenderer meshRenderer;


    protected override void Awake()
    {
        base.Awake();
        
        meshRenderer = GetComponent<MeshRenderer>();
        UpdateVisuals();
    }

    public override string GetInteractionPrompt()
    {
        if (currentState == State.Dirty) return "Hold Click to Scrap Surface";
        return "Press E to Change Style";
    }

    public override void OnInteract(PlayerInteraction player)
    {
        // MODE 1: SCRAPING (Requires Holding Left Click)
        if (currentState == State.Dirty)
        {
            // We check if the player is actually pressing the Primary Action (Left Click)
            if (player.primaryInput.action.IsPressed())
            {
                lastInteractTime = Time.time;
                scrapTimer += Time.deltaTime;

                if (scrapTimer >= scrapTime)
                {
                    currentState = State.Clean;
                    UpdateVisuals();
                }
            }
        }
        // MODE 2: PAINTING (Requires Pressing E or Click)
        else
        {
            // We only want to cycle if the button was JUST pressed, not held.
            // But OnInteract is called continuously for Primary Hold.
            // So we rely on the PlayerInteraction's "E" key for this, OR use a cooldown.
            
            // Let's use the cooldown approach as it's robust:
            if (Time.time > lastInteractTime + 0.2f)
            {
                CyclePattern();
                lastInteractTime = Time.time;
            }
        }
    }

    // This is called specifically when 'E' is pressed once (if mapped that way in PlayerInteraction)
    // For this specific system, we might want to separate "Hold" logic from "Click" logic cleanly.
    // But sticking to your interface:
    
    public void CyclePattern()
    {
        // Simple debounce to prevent cycling 60 times a second if held
        if (Time.time - lastInteractTime < 0.2f) return; 
        lastInteractTime = Time.time;

        currentPatternIndex++;
        if (currentPatternIndex >= availablePatterns.Length) currentPatternIndex = 0;
        
        meshRenderer.material = availablePatterns[currentPatternIndex];
    }

    public override void OnAltInteract(PlayerInteraction player) 
    {
        // Right click could cycle backwards
    }


    protected override void Update()
    {
        base.Update();
        
        // Reset Scrap Timer if player lets go
        if (currentState == State.Dirty && Time.time - lastInteractTime > 0.1f)
        {
            scrapTimer = 0;
        }
    }

    private void UpdateVisuals()
    {
        if (currentState == State.Dirty)
        {
            meshRenderer.material = dirtyMaterial;
        }
        else
        {
            meshRenderer.material = defaultCleanMaterial;
        }
    }
}