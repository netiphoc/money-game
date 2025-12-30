using System;
using Data;
using Systems;
using UnityEngine;
using TMPro;
using Utilities;

public class RoomUnlocker : BaseInteractable
{
    [Header("Settings")] 
    [SerializeField] private RoomDataSO roomDataSo;
    [SerializeField] private GymRoom gymRoom;

    [Header("References")]
    public GameObject doorBarrier;
    public GameObject forSaleSign;
    public TMP_Text priceText;
    public TMP_Text levelText;
    public GameObject lockIcon;


    protected override void Start()
    {
        base.Start();
        
        UpdateVisuals();
        GameManager.Instance.OnLevelChanged += OnLevelChanged;
        GameManager.Instance.OnMoneyChanged += OnMoneyChanged;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        
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

    public override string GetInteractionPrompt()
    {
        if (gymRoom.IsUnlocked) return "";

        // Check GLOBAL Player Level
        if (GameManager.Instance.playerLevel < roomDataSo.requiredGymLevel)
        {
            return $"Requires Gym Level {roomDataSo.requiredGymLevel}";
        }

        return $"Press E to Buy Room ({roomDataSo.unlockCost.ToMoneyFormat()})";
    }

    public override void OnInteract(PlayerInteraction player)
    {
        if (gymRoom.IsUnlocked) return;

        // 1. Check Player Level
        if (GameManager.Instance.playerLevel < roomDataSo.requiredGymLevel) return;

        // 2. Check Money
        if (GameManager.Instance.TrySpendMoney(roomDataSo.unlockCost))
        {
            Unlock();
        }
    }

    public override void OnAltInteract(PlayerInteraction player) { }

    private void Unlock()
    {
        doorBarrier.SetActive(false);
        forSaleSign.SetActive(false);
        gymRoom.UnlockRoom(gymRoom);
    }

    private void UpdateVisuals()
    {
        levelText.SetText($"Requires Level: {roomDataSo.requiredGymLevel}");
        if (priceText) priceText.text = roomDataSo.unlockCost.ToMoneyFormat();
        
        // Show Lock Icon if Player Level is too low
        if (lockIcon)
        {
            lockIcon.SetActive(GameManager.Instance.playerLevel < roomDataSo.requiredGymLevel);
        }
    }
}