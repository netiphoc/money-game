using UnityEngine;
using UnityEngine.UI;

namespace UI.RecruitBoxer
{
    public class UIRecruitBoxer : BaseUI
    {
        [SerializeField] private UIRecruitBoxerSlot uiRecruitBoxerSlot;
        [SerializeField] private Transform containerSlot;

        [Header("Button")] [SerializeField] private Button closeButton;
        
        [Header("Boxer")] 
        [SerializeField] private BoxerController boxerPrefab;
        [SerializeField] private BoxerData[] boxers;
        
        private GymRoom _gymRoom;
        
        protected override void Awake()
        {
            base.Awake();
            InitRecruit();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            closeButton.onClick.AddListener(OnButtonClickedClose);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            closeButton.onClick.AddListener(OnButtonClickedClose);
        }
        
        private void OnButtonClickedClose()
        {
            UIManager.Instance.ShowGymUnlock(_gymRoom,false);
        }

        private void InitRecruit()
        {
            foreach (var boxer in boxers)
            {
                UIRecruitBoxerSlot slot = Instantiate(uiRecruitBoxerSlot, containerSlot);
                slot.SetBoxer(boxer);
                slot.OnClickUnlock = OnClickUnlock;
            }
        }

        private void OnClickUnlock(BoxerData boxer)
        {
            if (_gymRoom.IsUnlocked) return;

            // 1. Check Player Level
            if (GameManager.Instance.playerLevel < _gymRoom.RoomDataSo.requiredGymLevel) return;

            // 2. Check Money
            if (GameManager.Instance.TrySpendMoney(_gymRoom.RoomDataSo.unlockCost))
            {
                _gymRoom.UnlockRoom();
            }
            
            if (GameManager.Instance.TrySpendMoney(_gymRoom.GetUpgradeCost()))
            {
                // 1. Spawn the Visual NPC
                BoxerController controller = Instantiate(boxerPrefab, _gymRoom.SpawnPoint);
            
                // 2. Initialize Controller
                controller.stats = boxer; // Apply the stats we just generated

                // 3. Assign to Room
                _gymRoom.assignedBoxer = controller;
                controller.AssignToRoom(_gymRoom);
            }
        }

        public void SetRecruitBoxer(GymRoom gymRoom)
        {
            _gymRoom = gymRoom;
        }
    }
}