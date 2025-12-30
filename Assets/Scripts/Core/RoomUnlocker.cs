using System;
using Data;
using Systems;
using UnityEngine;
using TMPro;
using UI;
using Utilities;

public class RoomUnlocker : BaseInteractable
{
    [Header("Settings")] 
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
        gymRoom.OnRoomUnlocked += OnRoomUnlocked;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        
        GameManager.Instance.OnLevelChanged -= OnLevelChanged;
        GameManager.Instance.OnMoneyChanged -= OnMoneyChanged;
        gymRoom.OnRoomUnlocked -= OnRoomUnlocked;
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
        if (GameManager.Instance.playerLevel < gymRoom.RoomDataSo.requiredGymLevel)
        {
            return $"Requires Gym Level {gymRoom.RoomDataSo.requiredGymLevel}";
        }

        return $"Press E to Buy Room ({gymRoom.RoomDataSo.unlockCost.ToMoneyFormat()})";
    }

    public override void OnInteract(PlayerInteraction player)
    {
        if(UIManager.Instance.UIRecruitBoxer.Visible) return;
        UIManager.Instance.ShowGymUnlock(gymRoom, true);
    }

    public override void OnAltInteract(PlayerInteraction player) { }

    private void OnRoomUnlocked(GymRoom obj)
    {
        doorBarrier.SetActive(false);
        forSaleSign.SetActive(false);
        UIManager.Instance.ShowGymUnlock(default, false);
    }

    private void UpdateVisuals()
    {
        levelText.SetText($"Requires Level: {gymRoom.RoomDataSo.requiredGymLevel}");
        if (priceText) priceText.text = gymRoom.RoomDataSo.unlockCost.ToMoneyFormat();
        
        // Show Lock Icon if Player Level is too low
        if (lockIcon)
        {
            lockIcon.SetActive(GameManager.Instance.playerLevel < gymRoom.RoomDataSo.requiredGymLevel);
        }
    }
}