using TMPro;
using UnityEngine;

namespace UI.Gameplay
{
    public class UIDay : BaseUI
    {
        [SerializeField] private TMP_Text textDay;

        protected override void Start()
        {
            base.Start();
            
            SetDay(GameManager.Instance.totalDays);
        }

        public void SetDay(int day)
        {
            textDay.SetText($"{day}");
        }
    }
}