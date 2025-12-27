using TMPro;
using UnityEngine;
using Utilities;

namespace UIs.Tablet
{
    public class UITabletButtonBalance : BaseUIButton
    {
        [SerializeField] private TMP_Text textBalance;
        
        public void SetBalance(double balance)
        {
            textBalance.SetText(balance.ToMoneyFormat());
        }
    }
}