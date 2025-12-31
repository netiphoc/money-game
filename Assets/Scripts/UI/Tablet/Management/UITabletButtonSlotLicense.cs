using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace UI.Tablet.Management
{
    public class UITabletButtonSlotLicense : BaseUIButton
    {
        [SerializeField] private TMP_Text textLicenseName;
        [SerializeField] private TMP_Text textLicenseCost;
        [SerializeField] private Image iconLicense;
        [SerializeField] private Button buttonBuy;
        
        [Header("Unlock Level")]
        [SerializeField] private TMP_Text textRequiredLevel;
        [SerializeField] private Transform lockGroup;

        public Action<LicenseSO> OnOwnedLicense;
        
        private LicenseSO _licenseSo;
        protected override void OnEnable()
        {
            base.OnEnable();
            buttonBuy.onClick.AddListener(OnButtonClickBuyLicense);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            buttonBuy.onClick.RemoveListener(OnButtonClickBuyLicense);
        }

        private void OnButtonClickBuyLicense()
        {
            if (!LicenseManager.Instance.CanBuyLicense(_licenseSo))
            {
                FloatingTextManager.Instance.ShowFixedText("Not enough money!", Color.red);
                return;
            }
            
            LicenseManager.Instance.BuyLicense(_licenseSo);
            
            SetOwned();
            OnOwnedLicense?.Invoke(_licenseSo);
        }

        public void SetLicense(LicenseSO licenseSo)
        {
            _licenseSo = licenseSo;
            //_licenseDataSo.isOwn = false; // Test
            
            // Level check
            SetLock(licenseSo.requiredLevel > GameManager.Instance.playerLevel);

            SetLicenseName(licenseSo.licenseName);
            SetLicenseIcon(licenseSo.icon);
            SetLicenseCost(licenseSo.cost);

            bool isUnlocked = LicenseManager.Instance.IsLicenseUnlocked(licenseSo);
            if (isUnlocked)
            {
                SetOwned();
            }
            else
            {
                buttonBuy.gameObject.SetActive(true);
            }
            
        }

        private void SetLicenseName(string licenseName)
        {
            textLicenseName.SetText(licenseName);
        }

        private void SetLicenseIcon(Sprite licenseIcon)
        {
            iconLicense.sprite = licenseIcon;
        }

        private void SetLicenseCost(int licenseCost)
        {
            textLicenseCost.SetText(licenseCost.ToMoneyFormat());
        }

        private void SetOwned()
        {
            textLicenseCost.SetText("OWNED");
            buttonBuy.gameObject.SetActive(false);
        }

        private void SetLock(bool isLock)
        {
            lockGroup.gameObject.SetActive(isLock);
            
            if (isLock)
            {
                textRequiredLevel.SetText($"Required level {_licenseSo.requiredLevel}");
            }
        }
    }
}