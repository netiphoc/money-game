using UI.Gameplay;
using UI.RecruitBoxer;
using UI.Tablet;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;

        [Header("Input")]
        public InputActionProperty toggleTabletInput;
        
        [field: SerializeField, Header("UI")] 
        public UITablet UITablet { get; private set; }
        [field: SerializeField] public UIBoxerStat UIBoxerStat { get; private set; }
        [field: SerializeField] public UIHoverKey UIHoverKey { get; private set; }
        [field: SerializeField] public UIRecruitBoxer UIRecruitBoxer { get; private set; }
        
        private bool _isTabletOpen;
        private bool _isRecruitOpen;

        private void Awake()
        {
            if (Instance == null) Instance = this;
        }

        private void OnEnable()
        {
            toggleTabletInput.action.Enable();
        }

        private void OnDisable()
        {
            toggleTabletInput.action.Disable();
        }

        private void Update()
        {
            if (toggleTabletInput.action.WasPerformedThisFrame())
            {
                if (_isRecruitOpen)
                {
                    ShowGymUnlock(default, false);
                    return;
                }

                ToggleTablet();
            }
        }
        
        private void ToggleTablet()
        {
            ShowTablet(!_isTabletOpen);
        }

        public void ShowTablet(bool isTabletOpen)
        {
            UIBoxerStat.SetVisible(false);
            UIHoverKey.SetVisible(false);
            
            _isTabletOpen = isTabletOpen;
            UITablet.SetVisible(isTabletOpen);

            SetUIInteractMode(isTabletOpen);
        }

        public void ShowGymUnlock(GymRoom gymRoom, bool show)
        {
            _isRecruitOpen = show;
            SetUIInteractMode(show);

            if (show)
            {            
                UIRecruitBoxer.SetRecruitBoxer(gymRoom);
                UIRecruitBoxer.SetVisible(true);
                return;
            }
            
            UIRecruitBoxer.SetVisible(false);
        }

        private void SetUIInteractMode(bool uiInteractMode)
        {
            GameManager.Instance.SetAllowPlayerInteraction(!uiInteractMode);
            
            if (uiInteractMode)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                return;
            }
            
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}