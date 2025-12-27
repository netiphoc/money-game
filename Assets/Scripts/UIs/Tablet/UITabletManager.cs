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
        [SerializeField] private ItemDataSO[] decorations;
        
        [Header("Room")]
        [SerializeField] private UITabletButtonRoom uiTabletButtonRoomPrefab;
        [SerializeField] private Transform containerRoom;
        
        [Header("Button")]
        [SerializeField] private Button buttonCart;
        [SerializeField] private Button buttonLicense;
        [SerializeField] private Button buttonProduct;
        [SerializeField] private Button buttonFurniture;
        [SerializeField] private Button buttonDecoration;
        [SerializeField] private Button buttonWorkers;
        [SerializeField] private Button buttonExpand;
        [SerializeField] private Button buttonFight;

        [field: SerializeField, Header("UI")] public UITabletCart UITabletCart { get; private set; }
        [field: SerializeField] public UITabletLicense UITabletLicense { get; private set; }
        [field: SerializeField] public UITabletProduct UITabletProduct { get; private set; }
        [field: SerializeField] public UITabletFurniture UITabletFurniture { get; private set; }
        [field: SerializeField] public UITabletDecoration UITabletDecoration { get; private set; }

        private BaseUI _activeTab;
        
        protected override void Awake()
        {
            base.Awake();
            InitData();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            buttonCart.onClick.AddListener(OnButtonClickedCart);
            buttonLicense.onClick.AddListener(OnButtonClickedLicense);
            buttonProduct.onClick.AddListener(OnButtonClickedProduct);
            buttonFurniture.onClick.AddListener(OnButtonClickedFurniture);
            buttonDecoration.onClick.AddListener(OnButtonClickedDecoration);
            buttonWorkers.onClick.AddListener(OnButtonClickedWorkers);
            buttonExpand.onClick.AddListener(OnButtonClickedExpand);
            buttonFight.onClick.AddListener(OnButtonClickedFight);
            UITabletLicense.OnOwnedLicense += OnOwnedLicense;
            UITabletProduct.OnAddItemToCart += OnAddItemToCart;
            UITabletFurniture.OnAddItemToCart += OnAddItemToCart;
            UITabletDecoration.OnAddItemToCart += OnAddItemToCart;
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            buttonCart.onClick.RemoveListener(OnButtonClickedCart);
            buttonLicense.onClick.RemoveListener(OnButtonClickedLicense);
            buttonProduct.onClick.RemoveListener(OnButtonClickedProduct);
            buttonFurniture.onClick.RemoveListener(OnButtonClickedFurniture);
            buttonDecoration.onClick.RemoveListener(OnButtonClickedDecoration);
            buttonWorkers.onClick.RemoveListener(OnButtonClickedWorkers);
            buttonExpand.onClick.RemoveListener(OnButtonClickedExpand);
            buttonFight.onClick.RemoveListener(OnButtonClickedFight);
            UITabletLicense.OnOwnedLicense -= OnOwnedLicense;
            UITabletProduct.OnAddItemToCart -= OnAddItemToCart;
            UITabletFurniture.OnAddItemToCart -= OnAddItemToCart;
            UITabletDecoration.OnAddItemToCart -= OnAddItemToCart;
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
            UITabletDecoration.SetDecorations(decorations);
        }

        private void SetActiveTab(BaseUI ui)
        {
            if (_activeTab != null)
            {
                _activeTab.SetVisible(false);
            }
            
            _activeTab = ui;
            _activeTab.SetVisible(true);
        }

        private void OnRoomDataChanged(RoomDataSO roomDataSo)
        {
            Debug.Log($"OnRoomDataChanged -> {roomDataSo.roomName}");
            UITabletLicense.SetLicenses(roomDataSo.licenses);
            UITabletProduct.SetProducts(roomDataSo.licenses.SelectMany(licenseDataSo => licenseDataSo.items).ToArray());
            UITabletFurniture.SetFurnitures(roomDataSo.furnitures);
        }
        
        private void OnButtonClickedCart()
        {
            UITabletCart.SetVisible(true);
        }

        private void OnButtonClickedLicense()
        {
            SetActiveTab(UITabletLicense);
        }

        private void OnButtonClickedProduct()
        {
            SetActiveTab(UITabletProduct);
        }

        private void OnButtonClickedFurniture()
        {
            SetActiveTab(UITabletFurniture);
        }

        private void OnButtonClickedDecoration()
        {
            SetActiveTab(UITabletDecoration);
        }

        private void OnButtonClickedWorkers()
        {
        }

        private void OnButtonClickedExpand()
        {
        }

        private void OnButtonClickedFight()
        {
        }
        
        private void OnOwnedLicense(LicenseDataSO licenseDataSo)
        {
            Debug.Log($"You owned new license: {licenseDataSo.licenseName}");
        }
        
        private void OnAddItemToCart(CartItemData cartItemData)
        {
            UITabletCart.AddItemCart(cartItemData);
        }
    }
}