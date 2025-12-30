using Systems;
using UnityEngine;
using TMPro;

public class RoomController : BaseInteractable
{
    [Header("Settings")]
    public int roomID;
    public int unlockCost = 500;
    public bool isUnlocked = false;

    [Header("References")]
    public GameObject unlockSign; // The "For Sale" visual with the price text
    public GameObject[] debrisObjects; // Trash inside (active only when unlocked initially)
    public GameObject[] propsToEnable; // Lights/Doors that turn on when unlocked
    public Collider constructionBarrier; // Invisible wall preventing entry

    protected override void Start()
    {
        base.Start();
        UpdateRoomState();
    }

    // Interaction Prompt
    public override string GetInteractionPrompt()
    {
        if (!isUnlocked) return $"Press E to Unlock Room {roomID} (${unlockCost})";
        return ""; // No prompt if already unlocked (unless you want a management menu)
    }

    public override void OnInteract(PlayerInteraction player)
    {
        if (!isUnlocked)
        {
            UnlockRoom();
        }
    }

    public override void OnAltInteract(PlayerInteraction player) { }

    private void UnlockRoom()
    {
        // 1. Check Money
        if (GameManager.Instance.TrySpendMoney(unlockCost))
        {
            isUnlocked = true;
            UpdateRoomState();
            // Play "Ka-ching" sound here
        }
        else
        {
            Debug.Log("Not enough money!");
            // Play "Error" sound
        }
    }

    private void UpdateRoomState()
    {
        if (isUnlocked)
        {
            if(unlockSign) unlockSign.SetActive(false); // Hide "For Sale" sign
            if(constructionBarrier) constructionBarrier.enabled = false; // Let player walk in

            // Enable lights/props
            foreach (var prop in propsToEnable) prop.SetActive(true);
            
            // Show Trash to be cleaned
            foreach (var trash in debrisObjects) trash.SetActive(true);
        }
        else
        {
            if(unlockSign) unlockSign.SetActive(true);
            if(constructionBarrier) constructionBarrier.enabled = true;
            
            foreach (var prop in propsToEnable) prop.SetActive(false);
            foreach (var trash in debrisObjects) trash.SetActive(false); // Hide trash inside locked room
        }
    }
}