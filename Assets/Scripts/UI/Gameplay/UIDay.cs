using TMPro;
using UnityEngine;

namespace UI.Gameplay
{
    public class UIDay : BaseUI
    {
        [SerializeField] private TMP_Text textDay;

        protected override void Awake()
        {
            base.Awake();
            textDay.SetText(string.Empty);
        }

        protected override void Start()
        {
            base.Start();
            Invoke(nameof(OnDayChanged), .5f);
        }

        private void OnDayChanged()
        {
            SetDay(GameManager.Instance.totalDays);
        }

        public void SetDay(int day)
        {
            textDay.SetText($"{day}");
        }
    }
}