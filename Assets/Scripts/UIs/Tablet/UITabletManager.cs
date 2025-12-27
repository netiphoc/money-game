using System.Collections.Generic;
using System.Linq;
using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UIs.Tablet
{
    public class UITabletManager : BaseUI
    {
        [Header("Data")]
        [SerializeField] private RoomDataSO[] tabletRoomData;
        [SerializeField] private ItemDataSO[] decorations;
        
        [Header("Room")]
        [SerializeField] private UITabletButtonRoom uiTabletButtonRoomPrefab;
        [SerializeField] private Transform containerRoom;
        
        [Header("Section Button")]
        [SerializeField] private TMP_Text textSectionName;
        [SerializeField] private Image iconSection;
        [Space]
        [SerializeField] private UITabletButtonSection buttonSectionRoom;
        [SerializeField] private UITabletButtonSection buttonSectionFood;
        [SerializeField] private UITabletButtonSection buttonSectionService;
        
        
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
        
        private readonly List<UITabletButtonRoom> _slotRoom = new List<UITabletButtonRoom>();

        protected override void Awake()
        {
            base.Awake();
            InitDataRoom();
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
            buttonSectionRoom.OnClickedSection = OnClickedSection;
            buttonSectionFood.OnClickedSection = OnClickedSection;
            buttonSectionService.OnClickedSection = OnClickedSection;
            
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

        private void InitDataRoom()
        {
            SectionDataSO loadSection = null;
            
            foreach (var data in tabletRoomData)
            {
                UITabletButtonRoom uiTabletButtonRoom = Instantiate(uiTabletButtonRoomPrefab, containerRoom);
                uiTabletButtonRoom.SetRoomData(data);
                uiTabletButtonRoom.OnClickRoomData = OnRoomDataChanged;
                _slotRoom.Add(uiTabletButtonRoom);
                
                if (loadSection == null)
                {
                    loadSection = data.sectionData;
                }
            }
            
            ShowSection(loadSection);
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

        private void OnClickedSection(SectionDataSO sectionDataSo)
        {
            ShowSection(sectionDataSo);
        }
        
        private void ShowSection(SectionDataSO sectionDataSo)
        {
            bool isFirst = false;
            foreach (var slot in _slotRoom)
            {
                bool active = slot.RoomDataSo.sectionData.Equals(sectionDataSo);
                slot.SetVisible(active);

                if (!isFirst && active)
                {
                    isFirst = true;
                    slot.OnRoomDataLoaded();
                }
            }
            
            textSectionName.SetText(sectionDataSo.sectionName);
            iconSection.sprite = sectionDataSo.icon;
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