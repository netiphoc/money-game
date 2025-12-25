using UnityEngine;

public class RoomPC : MonoBehaviour, IInteractable
{
    [Header("Link")]
    public GymRoom linkedRoom; // Drag the parent GymRoom object here

    public string GetInteractionPrompt()
    {
        return "Press E to Open Training Manager";
    }

    public void OnInteract(PlayerInteraction player)
    {
        // Open the UI and pass the Room Data
        TrainingUIManager.Instance.OpenMenu(linkedRoom);
        
        // Unlock cursor so player can click UI buttons
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void OnAltInteract(PlayerInteraction player) { }
}