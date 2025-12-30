// This interface defines what any interactive object MUST have

using UI;

public interface IInteractable
{
    // The text that appears on UI (e.g., "Press E to Open", "Hold Click to Carry")
    string GetInteractionPrompt();
    BaseUI GetUI();

    // What happens when the player Left Clicks / Presses E
    void OnInteract(PlayerInteraction player);

    // What happens when the player Right Clicks (Optional, e.g., Throwing/Place Back)
    void OnAltInteract(PlayerInteraction player);
}