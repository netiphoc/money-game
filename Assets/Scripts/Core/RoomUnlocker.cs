using Systems;
using UnityEngine;
using UI;
using UI.RecruitBoxer;
using Utilities;

public class RoomUnlocker : BaseInteractable
{
    [Header("Settings")] 
    [SerializeField] private GymRoom gymRoom;
    [SerializeField] private UIRecruitBoxerUnlocker uiRecruitBoxerUnlocker;

    [Header("References")]
    public GameObject doorBarrier;

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
        // Check GLOBAL Player Level
        if (GameManager.Instance.playerLevel < gymRoom.RoomDataSo.requiredGymLevel) return;
        if(UIManager.Instance.UIRecruitBoxer.Visible) return;
        UIManager.Instance.UIRecruitBoxer.SetRecruitBoxer(gymRoom);
        UIManager.Instance.ShowUI(UIManager.Instance.UIRecruitBoxer);
    }

    public override void OnAltInteract(PlayerInteraction player) { }

    private void OnRoomUnlocked(GymRoom obj)
    {
        doorBarrier.SetActive(false);
        uiRecruitBoxerUnlocker.SetVisible(false);
        UIManager.Instance.CloseUI();
    }

    private void UpdateVisuals()
    {
        bool isCanUnlock = GameManager.Instance.playerLevel >= gymRoom.RoomDataSo.requiredGymLevel;
        if (isCanUnlock)
        {
            uiRecruitBoxerUnlocker.SetUnlock();
            uiRecruitBoxerUnlocker.SetPrice(gymRoom.RoomDataSo.unlockCost);
        }
        else
        {
            uiRecruitBoxerUnlocker.SetLockLevel(gymRoom.RoomDataSo.requiredGymLevel);
        }
    }
}