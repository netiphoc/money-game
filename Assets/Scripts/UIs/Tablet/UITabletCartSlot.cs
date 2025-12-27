using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace UIs.Tablet
{
    public class UITabletCartSlot : BaseUI
    {
        private const int MinCartItem = 0;
        private const int MaxCartItem = 99;
        
        [SerializeField] private Image iconItem;
        [SerializeField] private TMP_Text textItemName;
        [SerializeField] private TMP_Text textItemCost;
        [SerializeField] private TMP_Text textItemAmount;
        [SerializeField] private Button buttonClose;
        [SerializeField] private Button buttonAddCount;
        [SerializeField] private Button buttonRemoveCount;

        public Action OnCartUpdate;
        
        public int ItemAmount { get; private set; }
        private ItemDataSO _itemDataSo;

        protected override void OnEnable()
        {
            base.OnEnable();
            buttonClose.onClick.AddListener(OnButtonClickedCancel);
            buttonAddCount.onClick.AddListener(OnButtonClickedAddCount);
            buttonRemoveCount.onClick.AddListener(OnButtonClickedRemoveCount);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            buttonClose.onClick.RemoveListener(OnButtonClickedCancel);
            buttonAddCount.onClick.RemoveListener(OnButtonClickedAddCount);
            buttonRemoveCount.onClick.RemoveListener(OnButtonClickedRemoveCount);
        }
        
        private void OnButtonClickedCancel()
        {
            SetItemAmount(0);
            SetVisible(false);
        }

        private void OnButtonClickedAddCount()
        {
            SetItemAmount(ItemAmount + 1);
        }

        private void OnButtonClickedRemoveCount()
        {
            SetItemAmount(ItemAmount - 1);
        }

        private void SetItemAmount(int count)
        {
            ItemAmount = Mathf.Clamp(count, MinCartItem, MaxCartItem);
            textItemAmount.SetText($"{ItemAmount}");

            OnCartUpdate?.Invoke();
            UpdateItemCost();
        }

        private void UpdateItemCost()
        {
            textItemCost.SetText(CalculateCost().ToMoneyFormat());
        }
        
        public void SetItem(ItemDataSO itemDataSo, int amount)
        {
            _itemDataSo = itemDataSo;
            iconItem.sprite = itemDataSo.icon;
            textItemName.SetText(itemDataSo.itemName);
            
            SetItemAmount(amount);
        }
        
        public void AddCount(int count)
        {
            SetVisible(true);
            SetItemAmount(ItemAmount + count);
        }

        public double CalculateCost()
        {
            return ItemAmount * _itemDataSo.cost * _itemDataSo.stackAmount;
        }
    }
}