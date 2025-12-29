using TMPro;
using UnityEngine;
using Utilities;

namespace UI.Gameplay
{
    public class UIBalance : BaseUI
    {
        [SerializeField] private TMP_Text textBalance;

        protected override void Start()
        {
            base.Start();
            GameManager.Instance.OnMoneyChanged += OnMoneyChanged;
            OnMoneyChanged(GameManager.Instance.currentMoney);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            GameManager.Instance.OnMoneyChanged -= OnMoneyChanged;
        }

        private void OnMoneyChanged(int balance)
        {
            textBalance.SetText(balance.ToMoneyFormat());
        }
    }
}