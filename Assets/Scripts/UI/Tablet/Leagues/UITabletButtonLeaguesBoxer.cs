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

        public Action<BoxerData> OnClickedBoxer;

        private BoxerData _boxerData;
        
        public void SetBoxer(BoxerData boxerData)
        {
            _boxerData = boxerData;
            
            //SetOpponentAvatar();
            SetName(boxerData.boxerName);
            SetLevel(boxerData.level);
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

        protected override void OnButtonClicked()
        {
            base.OnButtonClicked();
            OnClickedBoxer?.Invoke(_boxerData);
        }
    }
}