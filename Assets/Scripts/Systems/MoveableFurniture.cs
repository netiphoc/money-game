using Systems;
using UnityEngine;

public class MoveableFurniture : BaseInteractable
{
    [Header("Data")]
    public PlaceableDataSO data; 
    
    // Hold Logic
    private float holdTimer;
    private const float HOLD_THRESHOLD = 0.5f; // How long to hold before moving
    private float lastInteractTime; 

    public override string GetInteractionPrompt()
    {
        /*
        if (holdTimer > 0)
        {
            int bars = (int)((holdTimer / HOLD_THRESHOLD) * 10);
            string progress = new string('|', bars);
            return $"Release to Cancel [{progress}]";
        }
        */
        
        return "Hold Left Click to Move";
    }

    public override void OnInteract(PlayerInteraction player)
    {
        /*
        // Inside OnInteract, before the threshold check:
        float scaleFactor = 1.0f - (holdTimer / HOLD_THRESHOLD) * 0.1f; // Shrinks to 90% size
        transform.localScale = Vector3.one * scaleFactor;
        */
        
        // Check if player is holding button
        if (player.IsPrimaryPressed())
        {
            lastInteractTime = Time.time;
            holdTimer += Time.deltaTime;

            // Trigger the Move after time passes
            if (holdTimer >= HOLD_THRESHOLD)
            {
                holdTimer = 0; // Reset
                
                // Call the Manager to turn this object into a Ghost
                PlacementManager.Instance.StartMovingExistingObject(this.gameObject, data);
            }
        }
    }

    public override void OnAltInteract(PlayerInteraction player) { } 

    protected override void Update()
    {
        // RESET LOGIC
        // If the player let go of the button (stopped interacting)
        if (Time.time - lastInteractTime > 0.1f)
        {
            holdTimer = 0;
        }
    }
}