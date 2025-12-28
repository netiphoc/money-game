using UnityEngine;
using TMPro;
using Utilities;

public class RoomUnlocker : MonoBehaviour, IInteractable
{
    [Header("Settings")]
    public int unlockCost;
    public int requiredGymLevel; // Changed from 'requiredBoxerLevel' to 'GymLevel'

    [Header("References")]
    public GameObject doorBarrier;
    public GameObject forSaleSign;
    public TMP_Text priceText;
    public GameObject lockIcon;

    private bool isUnlocked = false;

    private void Start()
    {
        UpdateVisuals();
    }

    public string GetInteractionPrompt()
    {
        if (isUnlocked) return "";

        // Check GLOBAL Player Level
        if (GameManager.Instance.playerLevel < requiredGymLevel)
        {
            return $"Requires Gym Level {requiredGymLevel}";
        }

        return $"Press E to Buy Room ({unlockCost.ToMoneyFormat()})";
    }

    public void OnInteract(PlayerInteraction player)
    {
        if (isUnlocked) return;

        // 1. Check Player Level
        if (GameManager.Instance.playerLevel < requiredGymLevel) return;

        // 2. Check Money
        if (GameManager.Instance.TrySpendMoney(unlockCost))
        {
            Unlock();
        }
    }

    public void OnAltInteract(PlayerInteraction player) { }

    private void Unlock()
    {
        isUnlocked = true;
        doorBarrier.SetActive(false);
        forSaleSign.SetActive(false);
    }

    private void UpdateVisuals()
    {
        if (priceText) priceText.text = unlockCost.ToMoneyFormat();
        
        // Show Lock Icon if Player Level is too low
        if (lockIcon)
        {
            lockIcon.SetActive(GameManager.Instance.playerLevel < requiredGymLevel);
        }
    }
}