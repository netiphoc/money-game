using TMPro;
using UnityEngine;
using Utilities;

namespace UI.Tablet.Management
{
    public class UITabletButtonBalance : BaseUIButton
    {
        [SerializeField] private TMP_Text textBalance;
        
        public void SetBalance(int balance)
        {
            textBalance.SetText(balance.ToMoneyFormat());
        }
    }
}