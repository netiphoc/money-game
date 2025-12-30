using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace UI.RecruitBoxer
{
    public class UIRecruitBoxerUnlocker : BaseUI
    {
        [SerializeField] private TMP_Text textLevelRequired;
        [SerializeField] private TMP_Text textRecruit;
        [SerializeField] private TMP_Text textPrice;
        [SerializeField] private Image icon;
        [SerializeField] private Sprite iconLocked;
        [SerializeField] private UIRecruitBoxerButton uiRecruitBoxerButton;
        
        private Sprite _iconUnlocked;

        protected override void Awake()
        {
            base.Awake();
            _iconUnlocked = icon.sprite;
        }

        public void SetPrice(int price)
        {
            textPrice.SetText($"{price.ToMoneyFormat()}");
        }

        public void SetLockLevel(int level)
        {
            textLevelRequired.SetText($"Required level {level}");
            textLevelRequired.gameObject.SetActive(true);
            textRecruit.gameObject.SetActive(false);
            
            icon.sprite = iconLocked;
            
            uiRecruitBoxerButton.SetEnabled(false);
            uiRecruitBoxerButton.SetButtonColor(Color.grey);
        }

        public void SetUnlock()
        {
            textLevelRequired.gameObject.SetActive(false);
            textRecruit.gameObject.SetActive(true);
            uiRecruitBoxerButton.SetEnabled(true);
            
            icon.sprite = _iconUnlocked;
            
            uiRecruitBoxerButton.ResetButtonColor();
        }
    }
}