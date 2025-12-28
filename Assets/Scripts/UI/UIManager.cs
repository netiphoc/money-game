using UI.Tablet;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        [Header("Input")]
        public InputActionProperty toggleTabletInput;
        
        [field: SerializeField, Header("UI")] 
        public UITablet UITablet { get; private set; }
        
        private bool _isTabletOpen;

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
            _isTabletOpen = !_isTabletOpen;
            UITablet.SetVisible(_isTabletOpen);

            if (_isTabletOpen)
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