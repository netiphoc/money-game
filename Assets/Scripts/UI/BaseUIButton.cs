using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI
{
    public abstract class BaseUIButton : BaseUI
    {
        private Button _activeButton;
        public UnityEvent onClick;

        private Image _buttonBg;
        private Color _buttonOriginColor;
        protected override void Awake()
        {
            base.Awake();
            TryGetComponent(out _activeButton);
            bool haveBg = TryGetComponent(out _buttonBg);
            if (haveBg)
            {
                _buttonOriginColor =_buttonBg.color;
            }
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

        public void SetEnabled(bool enable)
        {
            _activeButton.enabled = enable;
        }

        public void SetButtonColor(Color color)
        {
            _buttonBg.color = color;
        }

        public void ResetButtonColor()
        {
            _buttonBg.color = _buttonOriginColor;
        }
    }
}