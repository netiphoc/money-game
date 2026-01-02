using TMPro;
using UnityEngine;

namespace UI.Tablet.Leagues
{
    public class UITabletButtonFight : BaseUIButton
    {
        [SerializeField] private TMP_Text textFightButton;
        [SerializeField] private Color colorFightNotAvailable = Color.red;
        
        [Header("LOCK")]
        [SerializeField] private Transform lockLevelGroup;
        [SerializeField] private TMP_Text textRequiredLevel;

        protected override void Awake()
        {
            base.Awake();
            SetEnabled(false);
        }

        public void SetNeedMoreStats()
        {
            SetEnabled(false);
            ResetButtonColor();
            
            textFightButton.SetText("[NOT AVAILABLE]");
        }
        
        public void SetFightAvailable(bool canFight)
        {
            SetEnabled(canFight);
            textFightButton.SetText(canFight ? "FIGHT" : "[IN FIGHT]");
            if (canFight)
            {
                ResetButtonColor();
            }
            else
            {
                SetButtonColor(Color.pink);
            }
        }

        public void SetWatchLive()
        {
            SetEnabled(true);
            textFightButton.SetText("WATCH LIVE!");
            SetButtonColor(colorFightNotAvailable);
        }

        public void SetLock(int level)
        {
            lockLevelGroup.gameObject.SetActive(true);
            textRequiredLevel.SetText($"(Lvl. {level})");
        }

        public void Unlock()
        {
            lockLevelGroup.gameObject.SetActive(false);
        }
    }
}