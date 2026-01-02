using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Gameplay
{
    public class UIBar : BaseUI
    {
        [SerializeField] private Image fillImage;
        [SerializeField] private TMP_Text textValue;

        public void SetValue(float min, float max)
        {
            fillImage.fillAmount = Mathf.Clamp01(min / max);
            textValue.SetText($"{min:F0} / {max:F0}");
        }
    }
}