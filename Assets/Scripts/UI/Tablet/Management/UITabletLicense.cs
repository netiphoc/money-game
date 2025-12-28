using System;

namespace UI.Tablet.Management
{
    public class UITabletLicense : BaseUITabletItemShop<UITabletButtonSlotLicense>
    {
        public event Action<LicenseSO> OnOwnedLicense;
        public LicenseSO[] LicenseDataSO { get; private set; } 

        public void SetLicenses(LicenseSO[] licenseData)
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