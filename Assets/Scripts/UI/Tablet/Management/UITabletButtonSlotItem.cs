using System;
using System.Text;
using Data;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utilities;

namespace UI.Tablet.Management
{
    public class UITabletButtonSlotItem : BaseUIButton, IPointerEnterHandler, IPointerExitHandler
    {
        private const int MinCount = 1;
        private const int MaxCount = 99;
        
        [SerializeField] private Image iconItem;
        [SerializeField] private TMP_Text textItemName;
        [SerializeField] private TMP_Text textUnitCost;
        [SerializeField] private TMP_Text textTotalCost;
        [SerializeField] private TMP_Text textAddToCartCount;
        [SerializeField] private TMP_Text textItemAmount;
        [SerializeField] private Button buttonBuy;
        [SerializeField] private Button buttonAddCount;
        [SerializeField] private Button buttonRemoveCount;
        
        [Header("Locked")]
        [SerializeField] private Transform hoverInfoTransform;
        [SerializeField] private TMP_Text textRequiredBoxerLevel;
        [SerializeField] private TMP_Text textItemStats;
        
        [Header("Locked")]
        [SerializeField] private Transform lockTransform;
        [SerializeField] private TMP_Text textRequiredLicense;

        public Action<CartItemData> OnAddItemToCart;
        
        private int _itemAmount;
        private int _cartCount;
        private int _unitCost;
        private ItemDataSO _itemDataSO;

        protected override void Awake()
        {
            base.Awake();
            _cartCount = MinCount;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            
            buttonBuy.onClick.AddListener(OnButtonClickAddToCart);
            buttonAddCount.onClick.AddListener(OnButtonClickAddItemCount);
            buttonRemoveCount.onClick.AddListener(OnButtonClickRemoveItemCount);
            
            hoverInfoTransform.gameObject.SetActive(false);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
            buttonBuy.onClick.RemoveListener(OnButtonClickAddToCart);
            buttonAddCount.onClick.RemoveListener(OnButtonClickAddItemCount);
            buttonRemoveCount.onClick.RemoveListener(OnButtonClickRemoveItemCount);
        }

        public void SetItem(ItemDataSO item)
        {
            _itemDataSO = item;
            
            SetItemName(item.itemName);
            SetItemIcon(item.icon);
            SetItemAmount(item.stackAmount);
            SetUnitCost(item.cost);
            SetRequiredBoxerLevel(item.requiredBoxerLevel);

            bool isUnlocked = LicenseManager.Instance.IsLicenseUnlocked(item.licenseSo);
            if (isUnlocked)
            {
                SetOwned();
            } 
            else
            {
                SetRequiredLicense(item.licenseSo.licenseName);
            }
            
            UpdateItemStats(isUnlocked);
        }

        private void SetOwned()
        {
            lockTransform.gameObject.SetActive(false);
        }

        private void SetRequiredLicense(string licenseName)
        {
            lockTransform.gameObject.SetActive(true);
            textRequiredLicense.SetText($"({licenseName})");   
        }
        
        private void SetItemName(string itemName)
        {
            textItemName.SetText(itemName);
        }

        private void SetItemIcon(Sprite itemIcon)
        {
            iconItem.sprite = itemIcon;
        }

        private void SetUnitCost(int unitCost)
        {
            _unitCost = unitCost;
            textUnitCost.SetText(unitCost.ToMoneyFormat());
            
            UpdateTotalCost();
        }

        private void SetItemAmount(int count)
        {
            _itemAmount = count;
            textItemAmount.SetText($"{count}");
            
            UpdateTotalCost();
        }

        private void OnButtonClickAddToCart()
        {
            FloatingTextManager.Instance.ShowFlyingIcon(
                _itemDataSO.icon, 
                iconItem.transform.position, 
                UIManager.Instance.UITablet.UITabletManager.buttonCart.transform.position);

            OnAddItemToCart?.Invoke(new CartItemData(_itemDataSO, _cartCount));
            
            SetCartCount(1);
        }

        private void OnButtonClickAddItemCount()
        {
            SetCartCount(_cartCount + 1);
        }

        private void OnButtonClickRemoveItemCount()
        {
            SetCartCount(_cartCount - 1);
        }

        private void SetCartCount(int count)
        {
            _cartCount = Mathf.Clamp(count, MinCount, MaxCount);
            textAddToCartCount.SetText($"{_cartCount}");
        }
        
        private int GetTotalCost()
        {
            return _unitCost * _itemAmount * _cartCount;
        }
        
        private void UpdateTotalCost()
        {
            textTotalCost.SetText(GetTotalCost().ToMoneyFormat());
        }

        private void UpdateItemStats(bool isUnlock)
        {
            if (!isUnlock)
            {
                textItemStats.gameObject.SetActive(false);
                return;
            }
            
            textItemStats.gameObject.SetActive(true);
            
            float str = 0;
            float agi = 0;
            float sta = 0;
            
            float hunger = 0;
            float sleep = 0;

            if (_itemDataSO.itemPrefab.TryGetComponent(out TrainingEquipment trainingEquipment))
            {
                str += trainingEquipment.strPerSecond;
                agi += trainingEquipment.agiPerSecond;
                sta += trainingEquipment.staPerSecond;
            }
            
            if (_itemDataSO.isConsumable)
            {
                hunger += _itemDataSO.hungerBonus;
                sleep += _itemDataSO.sleepBonus;
                str += _itemDataSO.strBonus;
                agi += _itemDataSO.agiBonus;
                sta += _itemDataSO.staBonus;
            }

            StringBuilder stats = new StringBuilder();
            
            string strFormat = str > 0 ? "+" : string.Empty;
            string agiFormat = agi > 0 ? "+" : string.Empty;
            string staFormat = sta > 0 ? "+" : string.Empty;
            string hungerFormat = hunger > 0 ? "+" : string.Empty;
            string sleepFormat = sleep > 0 ? "+" : string.Empty;
            
            if (str != 0f) stats.Append($"STR {strFormat}{str:F1}");
            if (agi != 0f) stats.Append($"\nAGI {agiFormat}{agi:F1}");
            if (sta != 0f) stats.Append($"\nSTA {staFormat}{sta:F1}");
            if (hunger != 0f) stats.Append($"\nHUNGER {hungerFormat}{hunger:F1}");
            if (sleep != 0f) stats.Append($"\nSLEEP {sleepFormat}{sleep:F1}");
            
            textItemStats.SetText(stats.ToString());
        }

        private void SetRequiredBoxerLevel(int level)
        {
            textRequiredBoxerLevel.SetText(level > 1 ? $"Required boxer level {level}" : string.Empty);
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            hoverInfoTransform.gameObject.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            hoverInfoTransform.gameObject.SetActive(false);
        }
    }
}