using TMPro;
using UnityEngine;
using Utilities;

namespace UI.Tablet.Management
{
    public class UITabletButtonCart : BaseUIButton
    {
        [SerializeField] private TMP_Text textCartItemCount;
        [SerializeField] private TMP_Text textTotalCost;

        protected override void Awake()
        {
            base.Awake();
            
            SetTotalCost(0);
        }

        public void SetCartItemCount(int count)
        {
            textCartItemCount.SetText($"{count}");
        }
        
        public void SetTotalCost(int balance)
        {
            textTotalCost.SetText(balance.ToMoneyFormat());
        }
    }
}