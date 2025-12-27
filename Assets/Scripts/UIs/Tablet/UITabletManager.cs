using System.Linq;
using Data;
using UnityEngine;
using UnityEngine.UI;

namespace UIs.Tablet
{
    public class UITabletManager : BaseUI
    {
        [Header("Data")]
        [SerializeField] private RoomDataSO[] tabletData;
        
        [Header("Room")]
        [SerializeField] private UITabletButtonRoom uiTabletButtonRoomPrefab;
        [SerializeField] private Transform containerRoom;
        
        [Header("Button")]
        [SerializeField] private Button buttonLicense;
        [SerializeField] private Button buttonProduct;

        [field: SerializeField, Header("UI")] public UITabletLicense UITabletLicense { get; private set; }
        [field: SerializeField] public UITabletProduct UITabletProduct { get; private set; }

        private BaseUI _activeTab;
        
        protected override void Awake()
        {
            base.Awake();
            InitData();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            buttonLicense.onClick.AddListener(OnButtonClickedLicense);
            buttonProduct.onClick.AddListener(OnButtonClickedProduct);
            UITabletLicense.OnOwnedLicense += OnOwnedLicense;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            buttonLicense.onClick.RemoveListener(OnButtonClickedLicense);
            buttonProduct.onClick.RemoveListener(OnButtonClickedProduct);
            UITabletLicense.OnOwnedLicense -= OnOwnedLicense;
        }

        private void InitData()
        {
            bool isFirstRoom = true;
            foreach (var data in tabletData)
            {
                UITabletButtonRoom uiTabletButtonRoom = Instantiate(uiTabletButtonRoomPrefab, containerRoom);
                uiTabletButtonRoom.SetRoomData(data);
                uiTabletButtonRoom.OnClickRoomData = OnRoomDataChanged;

                if (isFirstRoom)
                {
                    isFirstRoom = false;
                    uiTabletButtonRoom.OnRoomDataLoaded();
                }
            }
            
            SetActiveTab(UITabletLicense);
        }

        public void OnRoomDataChanged(RoomDataSO roomDataSo)
        {
            Debug.Log($"OnRoomDataChanged -> {roomDataSo.roomName}");
            UITabletLicense.SetLicenses(roomDataSo.licenses);
            UITabletProduct.SetProducts(roomDataSo.licenses
                .SelectMany(licenseDataSo => licenseDataSo.items)
                .ToArray());
        }
        
        private void OnButtonClickedLicense()
        {
            SetActiveTab(UITabletLicense);
        }

        private void OnButtonClickedProduct()
        {
            SetActiveTab(UITabletProduct);
        }

        public void SetActiveTab(BaseUI ui)
        {
            if (_activeTab != null)
            {
                _activeTab.SetVisible(false);
            }
            
            _activeTab = ui;
            _activeTab.SetVisible(true);
        }

        private void OnOwnedLicense(LicenseDataSO licenseDataSo)
        {
            //UITabletProduct.SetProducts(licenseDataSo.items);
            Debug.Log($"You owned new license: {licenseDataSo.licenseName}");
        }
    }
}