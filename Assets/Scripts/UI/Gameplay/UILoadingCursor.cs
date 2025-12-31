using UnityEngine;
using UnityEngine.UI;

namespace UI.Gameplay
{
    public class UILoadingCursor : BaseUI
    {
        [SerializeField] private Image fillImage;

        public void SetProgress(float value)
        {
            fillImage.fillAmount = Mathf.Clamp01(value);
        }
    }
}