using System.Collections;
using TMPro;
using UnityEngine;
using Utilities;

namespace UI.Tablet.Management
{
    public class UITabletButtonCart : BaseUIButton
    {
        [SerializeField] private TMP_Text textCartItemCount;
        [SerializeField] private TMP_Text textTotalCost;
        [SerializeField] private float delay;
        
        private Coroutine _sumBalanceCoroutine;
        private WaitForSeconds _waitForSeconds;
        
        protected override void Awake()
        {
            base.Awake();
            
            SetTotalCost(0);
            _waitForSeconds = new WaitForSeconds(delay);
        }

        public void SetCartItemCount(int count)
        {
            OnCartCountChanged(count);
        }
        
        public void SetTotalCost(int balance)
        {
            textTotalCost.SetText(balance.ToMoneyFormat());
        }
        

        private void OnCartCountChanged(int count)
        {
            if (_sumBalanceCoroutine != null)
            {
                StopCoroutine(_sumBalanceCoroutine);
            }
            
            _sumBalanceCoroutine = StartCoroutine(UpdateMoneyCoroutine(count));
        }

        private IEnumerator UpdateMoneyCoroutine(int count)
        {
            yield return _waitForSeconds;
            
            textCartItemCount.SetText($"{count}");
            textCartItemCount.DoTextPunch();
        }
    }
}