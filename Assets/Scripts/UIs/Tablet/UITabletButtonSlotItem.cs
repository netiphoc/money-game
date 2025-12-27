using System;
using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace UIs.Tablet
{
    public class UITabletButtonSlotItem : BaseUIButton
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

           if (item.licenseDataSo.isOwned)
           {
               SetOwned();
           } 
           else
           {
               SetRequiredLicense(item.licenseDataSo.licenseName);
           }
        }

        private void SetOwned()
        {
            lockTransform.gameObject.SetActive(false);
        }

        private void SetRequiredLicense(string licenseName)
        {
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
    }
}