using System;
using Data;
using UnityEngine;

namespace UIs.Tablet
{
    public abstract class BaseUITabletItemShop<TButtonType> : BaseUI where TButtonType : BaseUIButton
    {
        [SerializeField] private TButtonType uiTabletButtonSlotItemPrefab;
        [SerializeField] private Transform containerItem;
        [SerializeField] private Sprite iconCart;
        public event Action<CartItemData> OnAddItemToCart;

        private PoolingUI<TButtonType> _poolingUI;

        private PoolingUI<TButtonType> GetPool()
        {
            return _poolingUI ??= new PoolingUI<TButtonType>(uiTabletButtonSlotItemPrefab);
        }

        protected void OnAddItemToCartEvent(CartItemData cartItemData)
        {
            OnAddItemToCart?.Invoke(cartItemData);
        }
        
        protected void RenderItems()
        {
            if(!isActiveAndEnabled) return;
            Debug.Log($"RenderProduct: {name}");
            GetPool().ClearPool();

            OnRenderItem();
        }

        protected TButtonType GetSlotItem()
        {
            return GetPool().Request(containerItem);
        }

        protected abstract void OnRenderItem();
        

        public override void SetVisible(bool visible)
        {
            base.SetVisible(visible);

            if (visible)
            {
                RenderItems();
            }
        }

        public Sprite GetIconCart()
        {
            return iconCart;
        }
    }
}