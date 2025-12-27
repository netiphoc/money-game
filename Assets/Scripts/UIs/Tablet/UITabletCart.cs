using System.Collections.Generic;
using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace UIs.Tablet
{
    public class UITabletCart : BaseUI
    {
        [SerializeField] private Button buttonClose;
        [SerializeField] private Button buttonCheckout;
        [SerializeField] private TMP_Text textTotalCost;
        [SerializeField] private UITabletButtonCart uiTabletButtonCart;
        
        [Header("Slot")]
        [SerializeField] private UITabletCartSlot uiTabletCartSlotPrefab;
        [SerializeField] private Transform containerCartSlot;

        private readonly Dictionary<ItemDataSO, UITabletCartSlot> _cart = new Dictionary<ItemDataSO, UITabletCartSlot>();
        private PoolingUI<UITabletCartSlot> _poolingUI;
        
        private double _totalCost;
        private int _itemAmount;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            buttonClose.onClick.AddListener(OnButtonClickedClose);
            buttonCheckout.onClick.AddListener(OnButtonClickedCheckout);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            buttonClose.onClick.RemoveListener(OnButtonClickedClose);
            buttonCheckout.onClick.RemoveListener(OnButtonClickedCheckout);
        }

        private void OnButtonClickedClose()
        {
            SetVisible(false);
        }

        private void OnButtonClickedCheckout()
        {
            foreach (var cart in _cart)
            {
                Debug.Log($"- {cart.Key.itemName} x{cart.Value.ItemAmount} | {cart.Value.CalculateCost().ToMoneyFormat()}");
            }
            
            Debug.Log($"CHECK OUT: x{_itemAmount} {_totalCost.ToMoneyFormat()}");
 
            ClearCart();
            SetVisible(false);
        }
        
        private PoolingUI<UITabletCartSlot> GetPool()
        {
            return _poolingUI ??= new PoolingUI<UITabletCartSlot>(uiTabletCartSlotPrefab);
        }

        public void AddItemCart(CartItemData cartItemData)
        {
            if (_cart.TryGetValue(cartItemData.Item, out var value))
            {
                value.AddCount(cartItemData.Amount);
            }
            else
            {
                UITabletCartSlot slot = GetPool().Request(containerCartSlot);
                slot.SetItem(cartItemData.Item, cartItemData.Amount);
                slot.OnCartUpdate = UpdateCart;
                _cart.Add(cartItemData.Item, slot);
            }
            
            UpdateCart();
        }

        private void ClearCart()
        {
            _cart.Clear();
            GetPool().ClearPool();
            
            UpdateCart();
        }

        private void UpdateCart()
        {
            _totalCost = 0;
            _itemAmount = 0;
            
            foreach (var slot in _cart.Values)
            {
                _totalCost += slot.CalculateCost();
                _itemAmount += slot.ItemAmount;
            }
            
            textTotalCost.SetText(_totalCost.ToMoneyFormat());
            uiTabletButtonCart.SetCartItemCount(_itemAmount);
            uiTabletButtonCart.SetTotalCost(_totalCost);
        }
    }
}