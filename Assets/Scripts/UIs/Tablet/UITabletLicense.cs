using System;

namespace UIs.Tablet
{
    public class UITabletLicense : BaseUITabletItemShop<UITabletButtonSlotLicense>
    {
        public event Action<LicenseDataSO> OnOwnedLicense;
        public LicenseDataSO[] LicenseDataSO { get; private set; } 

        public void SetLicenses(LicenseDataSO[] licenseData)
        {
            LicenseDataSO = licenseData;
            RenderItems();
        }

        protected override void OnRenderItem()
        {
            foreach (var licenseDataSo in LicenseDataSO)
            {
                UITabletButtonSlotLicense uiTabletButtonSlot = GetSlotItem();
                uiTabletButtonSlot.SetLicense(licenseDataSo);
                uiTabletButtonSlot.OnOwnedLicense = OnOwnedLicense;
            }
        }
    }
}