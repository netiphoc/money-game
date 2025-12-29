using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Gameplay
{
    public class UILevelBoxer : BaseUI
    {
        [SerializeField] private Image fillExp;
        [SerializeField] private TMP_Text textLevel;
        [SerializeField] private TMP_Text textExpRequired;

        private BoxerController _boxerController;
        
        public void InitBoxer(BoxerController boxerController)
        {
            if (_boxerController != null)
            {
                _boxerController.stats.OnExpChanged -= OnExpChanged;
                _boxerController.stats.OnLevelChanged -= OnLevelChanged;
            }
            
            _boxerController = boxerController;
            _boxerController.stats.OnExpChanged += OnExpChanged;
            _boxerController.stats.OnLevelChanged += OnLevelChanged;
            
            OnExpChanged(_boxerController.stats.currentXP);
            OnLevelChanged(_boxerController.stats.level);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _boxerController.stats.OnExpChanged -= OnExpChanged;
            _boxerController.stats.OnLevelChanged -= OnLevelChanged;
        }

        private void OnExpChanged(float xp)
        {
            float xpToNextLevel = _boxerController.stats.xpToNextLevel;
            float value = Mathf.Clamp01(xp / xpToNextLevel);
            fillExp.fillAmount = value;
            
            textExpRequired.SetText($"{xp:F0} / {xpToNextLevel:F0}");
        }
        
        private void OnLevelChanged(int level)
        {
            textLevel.SetText($"{level}");
        }
    }
}