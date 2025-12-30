using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Gameplay
{
    public class UIBoxerStatBar : BaseUI
    {
        [SerializeField] private TMP_Text textValue;
        [SerializeField] private Image fillImage;

        protected override void Awake()
        {
            base.Awake();
            fillImage.fillAmount = 1f;
        }

        public void SetBar(float value, float maxValue)
        {
            textValue.SetText($"{value:F0} / {maxValue}");
            
            float factor = Mathf.Clamp01(value / maxValue);
            fillImage.fillAmount = factor;
        }
    }
}