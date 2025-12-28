using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Tablet.Leagues
{
    public class UITabletButtonLeaguesBoxer : BaseUIButton
    {
        [SerializeField] private Image iconAvatar;
        [SerializeField] private TMP_Text textName;
        [SerializeField] private TMP_Text textLevel;
        [SerializeField] private TMP_Text textStrength;
        [SerializeField] private TMP_Text textAgility;
        [SerializeField] private TMP_Text textStamina;

        public Action<BoxerController> OnClickedBoxer;

        private BoxerController _boxerController;
        
        public void SetBoxer(BoxerController boxerData)
        {
            _boxerController = boxerData;
            
            //SetOpponentAvatar();
            SetName(boxerData.stats.boxerName);
            SetLevel(boxerData.stats.level);
            SetStrength(Mathf.FloorToInt(boxerData.stats.strength));
            SetAgility(Mathf.FloorToInt(boxerData.stats.agility));
            SetStamina(Mathf.FloorToInt(boxerData.stats.stamina));
        }
        
        private void SetOpponentAvatar(Sprite sprite)
        {
            iconAvatar.sprite = sprite;
        }
        
        private void SetName(string boxerName)
        {
            textName.SetText($"{boxerName}");
        }
        
        private void SetLevel(int level)
        {
            textLevel.SetText($"Lvl. {level}");
        }
        
        private void SetStrength(int value)
        {
            textStrength.SetText($"{value}");
        }
        
        private void SetAgility(int value)
        {
            textAgility.SetText($"{value}");
        }
        
        private void SetStamina(int value)
        {
            textStamina.SetText($"{value}");
        }

        protected override void OnButtonClicked()
        {
            base.OnButtonClicked();
            OnClickedBoxer?.Invoke(_boxerController);
        }
    }
}