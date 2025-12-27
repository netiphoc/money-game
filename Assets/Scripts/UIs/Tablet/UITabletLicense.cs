using System;
using UnityEngine;

namespace UIs.Tablet
{
    public class UITabletLicense : BaseUI
    {
        [Header("License")]
        [SerializeField] private UITabletButtonSlotLicense uiTabletButtonSlotLicense;
        [SerializeField] private Transform containerLicense;
        public event Action<LicenseDataSO> OnOwnedLicense;
        public LicenseDataSO[] LicenseDataSO { get; private set; } 

        private PoolingUI<UITabletButtonSlotLicense> _poolingUI;
        private PoolingUI<UITabletButtonSlotLicense> GetPool()
        {
            return _poolingUI ??= new PoolingUI<UITabletButtonSlotLicense>(uiTabletButtonSlotLicense);
        }
        public void SetLicenses(LicenseDataSO[] licenseData)
        {
            LicenseDataSO = licenseData;
            RenderLicenses();
        }

        public void RenderLicenses()
        {
            if(!isActiveAndEnabled) return;
            Debug.Log($"RenderLicenses: {name}");
            GetPool().ClearPool();
            
            foreach (var licenseDataSo in LicenseDataSO)
            {
                UITabletButtonSlotLicense uiTabletButtonSlot = GetPool().Request(containerLicense);
                uiTabletButtonSlot.SetLicense(licenseDataSo);
                uiTabletButtonSlot.OnOwnedLicense = OnOwnedLicense;
            }
        }

        public override void SetVisible(bool visible)
        {
            base.SetVisible(visible);

            if (visible)
            {
                RenderLicenses();
            }
        }
    }
}