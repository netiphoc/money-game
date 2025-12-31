using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Gameplay
{
    public class UILevel : BaseUI
    {
        [SerializeField] private Image fillExp;
        [SerializeField] private TMP_Text textLevel;

        protected override void Start()
        {
            base.Start();
            GameManager.Instance.OnExpChanged += OnExpChanged;
            GameManager.Instance.OnLevelChanged += OnLevelChanged;
            OnExpChanged(GameManager.Instance.playerXP, GameManager.Instance.xpToNextLevel);
            OnLevelChanged(GameManager.Instance.playerLevel);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            GameManager.Instance.OnExpChanged -= OnExpChanged;
            GameManager.Instance.OnLevelChanged -= OnLevelChanged;
        }

        private void OnExpChanged(float xp, float xpToNextLevel)
        {
            float value = Mathf.Clamp01(xp / GameManager.Instance.xpToNextLevel);
            fillExp.fillAmount = value;
        }
        
        private void OnLevelChanged(int level)
        {
            textLevel.SetText($"{level}");
        }
    }
}