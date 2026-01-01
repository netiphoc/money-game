using Systems;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public enum KeyIcon
    {
        ALERT = -1, 
        COIN = -2, 
        MOUSE_LEFT_CLICK = 0, 
        MOUSE_RIGHT_CLICK = 1,
        G = 2, 
        X = 3, 
    }

    public class UIHoverKeySlot : BaseUI
    {
        [SerializeField] private TMP_Text textHover;
        [SerializeField] private Image iconKey;
        [SerializeField] private Sprite[] icons;
        [SerializeField] private Sprite iconAlert;
        [SerializeField] private Sprite iconMoney;
        
        public void SetInteractionPromptData(InteractionPromptData data)
        {
            iconKey.sprite = GetIcon(data.Icon);
            textHover.SetText(string.IsNullOrEmpty(data.Prompt) ? data.RealTimePrompt() : data.Prompt);    
        }

        private Sprite GetIcon(KeyIcon icon)
        {
            switch (icon)
            {
                case KeyIcon.ALERT:
                    return iconAlert;
                case KeyIcon.COIN:
                    return iconMoney;
            }
            
            return icons[(int)icon];
        }
    }
}