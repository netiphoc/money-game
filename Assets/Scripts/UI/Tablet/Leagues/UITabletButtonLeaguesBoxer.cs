using System;
using TMPro;
using UI.Gameplay;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

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
        [SerializeField] private UILevelBoxer uiLevelBoxer;

        public Action<BoxerController> OnClickedBoxer;

        private BoxerController _boxerController;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            GameManager.Instance.GameTimeManager.OnGameMinuteTick += OnGameMinuteTick;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            GameManager.Instance.GameTimeManager.OnGameMinuteTick += OnGameMinuteTick;
        }

        private void OnGameMinuteTick(string obj)
        {
            if(!_boxerController) return;
            SetStrength(Mathf.FloorToInt(_boxerController.stats.strength));
            SetAgility(Mathf.FloorToInt(_boxerController.stats.agility));
            SetStamina(Mathf.FloorToInt(_boxerController.stats.stamina));
        }
        
        public void SetBoxer(BoxerController boxerController)
        {
            uiLevelBoxer.InitBoxer(boxerController);
            
            _boxerController = boxerController;
            
            //SetOpponentAvatar();
            SetName(boxerController.stats.boxerName);
            SetLevel(boxerController.stats.level);
            SetStrength(Mathf.FloorToInt(boxerController.stats.strength));
            SetAgility(Mathf.FloorToInt(boxerController.stats.agility));
            SetStamina(Mathf.FloorToInt(boxerController.stats.stamina));
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
            textStrength.SetText($"{value.ToPowerFormat()}");
        }
        
        private void SetAgility(int value)
        {
            textAgility.SetText($"{value.ToPowerFormat()}");
        }
        
        private void SetStamina(int value)
        {
            textStamina.SetText($"{value.ToPowerFormat()}");
        }

        protected override void OnButtonClicked()
        {
            base.OnButtonClicked();
            OnClickedBoxer?.Invoke(_boxerController);
        }
    }
}