using UnityEngine.Events;
using UnityEngine.UI;

namespace UI
{
    public abstract class BaseUIButton : BaseUI
    {
        private Button _activeButton;
        public UnityEvent onClick;

        protected override void Awake()
        {
            base.Awake();
            TryGetComponent(out _activeButton);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            
            if (_activeButton) _activeButton.onClick.AddListener(OnButtonClicked);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
            if (_activeButton) _activeButton.onClick.RemoveListener(OnButtonClicked);
        }

        protected virtual void OnButtonClicked()
        {
            onClick?.Invoke();
        }
    }
}