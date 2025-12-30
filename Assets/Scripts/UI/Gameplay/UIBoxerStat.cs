using TMPro;
using UnityEngine;

namespace UI.Gameplay
{
    public class UIBoxerStat : BaseUI
    {
        [SerializeField] private TMP_Text textName;
        [SerializeField] private TMP_Text textStr;
        [SerializeField] private TMP_Text textAgil;
        [SerializeField] private TMP_Text textSta;
        [SerializeField] private TMP_Text textLevel;
        [SerializeField] private TMP_Text textNeededExp;
        [SerializeField] private UIBoxerStatBar expBar;
        [SerializeField] private UIBoxerStatBar hungerBar;
        [SerializeField] private UIBoxerStatBar sleepBar;

        private const float MaxValue = 100f;
        private BoxerController _boxerController;
        
        public void SetBoxerController(BoxerController boxerController)
        {
            _boxerController = boxerController;
        }

        public override void SetVisible(bool visible)
        {
            base.SetVisible(visible);

            if (visible && _boxerController)
            {
                textName.SetText($"{_boxerController.stats.boxerName}");
                textStr.SetText($"STR: {_boxerController.stats.strength:F0}");
                textAgil.SetText($"AGIL: {_boxerController.stats.agility:F0}");
                textSta.SetText($"STA: {_boxerController.stats.stamina:F0}");
                textLevel.SetText($"LEVEL: {_boxerController.stats.level}");
                expBar.SetBar(Mathf.Min(_boxerController.stats.currentXP, _boxerController.stats.xpToNextLevel), _boxerController.stats.xpToNextLevel);
                hungerBar.SetBar(Mathf.Min(_boxerController.stats.hunger, MaxValue), MaxValue);
                sleepBar.SetBar(Mathf.Min(_boxerController.stats.sleep, MaxValue), MaxValue);

                if (textNeededExp)
                {
                    textNeededExp.SetText($"Needed {_boxerController.stats.xpToNextLevel - _boxerController.stats.currentXP} exp to next level");
                }
            }
        }
    }
}