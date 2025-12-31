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
            
            foreach (var boxer in boxers)
            {
                UIRecruitBoxerSlot slot = Instantiate(uiRecruitBoxerSlot, containerSlot);
                boxer.InitStats();
                slot.SetBoxer(boxer);
                slot.OnClickUnlock = OnClickUnlock;
            }
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
        
        public void SetRecruitBoxer(GymRoom gymRoom)
        {
            _gymRoom = gymRoom;
        }
        
        private void OnButtonClickedClose()
        {
            UIManager.Instance.CloseUI();
        }

        private void OnClickUnlock(BoxerData boxer)
        {
            if (_gymRoom.IsUnlocked ||
                // 1. Check Player Level
                GameManager.Instance.playerLevel < _gymRoom.RoomDataSo.requiredGymLevel ||
                // 2. Check Money
                !GameManager.Instance.TrySpendMoney(_gymRoom.RoomDataSo.unlockCost, SpendType.UPGRADE))
            {
                Debug.Log("No Money");
                FloatingTextManager.Instance.ShowFixedText("Not enough money!", Color.red);
                OnButtonClickedClose();
                return;
            }

            _gymRoom.UnlockRoom();
                
            // 1. Spawn the Visual NPC
            BoxerController controller = Instantiate(boxerPrefab, _gymRoom.SpawnPoint);
            
            // 2. Initialize Controller
            controller.stats = boxer; // Apply the stats we just generated

            // 3. Assign to Room
            _gymRoom.assignedBoxer = controller;
            controller.AssignToRoom(_gymRoom);
        }
    }
}