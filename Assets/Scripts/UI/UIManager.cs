using UI.Gameplay;
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
        
        private bool _isTabletOpen;

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

            GameManager.Instance.SetAllowPlayerInteraction(!isTabletOpen);

            if (isTabletOpen)
            {
                // Unlock Cursor so we can click
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                // Pause Player Interaction? (Optional, but good practice)
                // FindObjectOfType<PlayerInteraction>().enabled = false; 
            }
            else
            {
                // Lock Cursor back
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            
                // FindObjectOfType<PlayerInteraction>().enabled = true;
            }
        }
    }
}