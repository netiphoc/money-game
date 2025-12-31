using System.Collections.Generic;
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
        [field: SerializeField] public UISummary UISummary { get; private set; }

        public bool LockUIInput { get; set; }
        
        private readonly List<BaseUI> _ui = new List<BaseUI>();
        
        private void Awake()
        {
            if (Instance == null) Instance = this;
            InitUI();
        }

        private void InitUI()
        {
            _ui.Add(UITablet);
            _ui.Add(UIBoxerStat);
            _ui.Add(UIHoverKey);
            _ui.Add(UIRecruitBoxer);
            _ui.Add(UIHoverKey);
            _ui.Add(UISummary);
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
                if(LockUIInput) return;
                ToggleTablet();
            }
        }
        
        public void ShowUI(BaseUI ui)
        {
            if(LockUIInput) return;
            foreach (var app in _ui)
            {
                app.SetVisible(app == ui);
            }

            SetUIInteractMode(true);
        }

        public void CloseUI()
        {
            foreach (var app in _ui)
            {
                if(!app.Visible) continue;
                app.SetVisible(false);
            }
            
            SetUIInteractMode(false);
        }

        private void ToggleTablet()
        {
            if (UITablet.Visible)
            {
                CloseUI();
            }
            else
            {
                ShowUI(UITablet); 
            }
        }

        public static void SetUIInteractMode(bool uiInteractMode)
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