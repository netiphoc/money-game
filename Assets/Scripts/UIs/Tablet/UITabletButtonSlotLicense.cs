using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace UIs.Tablet
{
    public class UITabletButtonSlotLicense : BaseUIButton
    {
        [SerializeField] private TMP_Text textLicenseName;
        [SerializeField] private TMP_Text textLicenseCost;
        [SerializeField] private Image iconLicense;
        [SerializeField] private Button buttonBuy;

        public Action<LicenseDataSO> OnOwnedLicense;
        
        private LicenseDataSO _licenseDataSo;
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
            _licenseDataSo.isOwned = true;
            SetOwned();
            OnOwnedLicense?.Invoke(_licenseDataSo);
        }

        public void SetLicense(LicenseDataSO licenseDataSo)
        {
            _licenseDataSo = licenseDataSo;
            //_licenseDataSo.isOwn = false; // Test
            
            SetLicenseName(licenseDataSo.licenseName);
            SetLicenseIcon(licenseDataSo.icon);
            SetLicenseCost(licenseDataSo.cost);
            
            if (licenseDataSo.isOwned)
            {
                SetOwned();
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
    }
}