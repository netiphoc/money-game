using System;
using Data;
using UnityEngine;
using TMPro;
using Utilities;

public class RoomUnlocker : MonoBehaviour, IInteractable
{
    [Header("Settings")] 
    [SerializeField] private RoomDataSO roomDataSo;

    [Header("References")]
    public GameObject doorBarrier;
    public GameObject forSaleSign;
    public TMP_Text priceText;
    public TMP_Text levelText;
    public GameObject lockIcon;
    public GameObject unlockGroup;

    private bool isUnlocked = false;

    private void Start()
    {
        UpdateVisuals();
        GameManager.Instance.OnLevelChanged += OnLevelChanged;
        GameManager.Instance.OnMoneyChanged += OnMoneyChanged;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnLevelChanged -= OnLevelChanged;
        GameManager.Instance.OnMoneyChanged -= OnMoneyChanged;
    }

    private void OnLevelChanged(int obj)
    {
        UpdateVisuals();
    }

    private void OnMoneyChanged(int obj)
    {
        UpdateVisuals();
    }

    public string GetInteractionPrompt()
    {
        if (isUnlocked) return "";

        // Check GLOBAL Player Level
        if (GameManager.Instance.playerLevel < roomDataSo.requiredGymLevel)
        {
            return $"Requires Gym Level {roomDataSo.requiredGymLevel}";
        }

        return $"Press E to Buy Room ({roomDataSo.unlockCost.ToMoneyFormat()})";
    }

    public void OnInteract(PlayerInteraction player)
    {
        if (isUnlocked) return;

        // 1. Check Player Level
        if (GameManager.Instance.playerLevel < roomDataSo.requiredGymLevel) return;

        // 2. Check Money
        if (GameManager.Instance.TrySpendMoney(roomDataSo.unlockCost))
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
        unlockGroup.gameObject.SetActive(true);
    }

    private void UpdateVisuals()
    {
        unlockGroup.gameObject.SetActive(false);
        
        levelText.SetText($"Requires Level: {roomDataSo.requiredGymLevel}");
        if (priceText) priceText.text = roomDataSo.unlockCost.ToMoneyFormat();
        
        // Show Lock Icon if Player Level is too low
        if (lockIcon)
        {
            lockIcon.SetActive(GameManager.Instance.playerLevel < roomDataSo.requiredGymLevel);
        }
    }
}