using System.Collections;
using TMPro;
using UnityEngine;
using Utilities;

namespace UI.Gameplay
{
    public class UIBalance : BaseUI
    {
        [SerializeField] private TMP_Text textBalance;
        [SerializeField] private float delay = 2f;

        private WaitForSeconds _waitForSeconds;
        private bool _init;
        
        protected override void Awake()
        {
            base.Awake();
            _waitForSeconds = new WaitForSeconds(delay);
        }

        protected override void Start()
        {
            base.Start();
            GameManager.Instance.OnMoneyChanged += OnMoneyChanged;
            //OnMoneyChanged(GameManager.Instance.currentMoney);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            GameManager.Instance.OnMoneyChanged -= OnMoneyChanged;
        }

        private int _sumBalance;
        private Coroutine _sumBalanceCoroutine;

        private void OnMoneyChanged(int balance)
        {
            if (!_init)
            {
                _init = true;
                textBalance.SetText(balance.ToMoneyFormat());
                return;
            }
            
            _sumBalance += balance;

            if (_sumBalanceCoroutine != null)
            {
                StopCoroutine(_sumBalanceCoroutine);
            }
            
            _sumBalanceCoroutine = StartCoroutine(UpdateMoneyCoroutine());
        }

        private IEnumerator UpdateMoneyCoroutine()
        {
            yield return _waitForSeconds;
            
            textBalance.SetText(_sumBalance.ToMoneyFormat());
            _sumBalance = 0;
        }
    }
}