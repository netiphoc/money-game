using TMPro;
using UnityEngine;
using Utilities;

namespace UIs.Tablet
{
    public class UITabletButtonCart : BaseUIButton
    {
        [SerializeField] private TMP_Text textCartItemCount;
        [SerializeField] private TMP_Text textTotalCost;

        public void SetCartItemCount(int count)
        {
            textCartItemCount.SetText($"{count}");
        }
        
        public void SetTotalCost(double balance)
        {
            textTotalCost.SetText(balance.ToMoneyFormat());
        }
    }
}