using UnityEngine;

namespace UIs.Tablet
{
    public abstract class BaseUITabletItemShop<TButtonType> : BaseUI where TButtonType : BaseUIButton
    {
        [SerializeField] private TButtonType uiTabletButtonSlotItemPrefab;
        [SerializeField] private Transform containerItem;
        
        private PoolingUI<TButtonType> _poolingUI;

        private PoolingUI<TButtonType> GetPool()
        {
            return _poolingUI ??= new PoolingUI<TButtonType>(uiTabletButtonSlotItemPrefab);
        }
        
        protected void RenderItems()
        {
            if(!isActiveAndEnabled) return;
            Debug.Log($"RenderProduct: {name}");
            GetPool().ClearPool();

            OnRenderItem();
        }

        public override void SetVisible(bool visible)
        {
            base.SetVisible(visible);

            if (visible)
            {
                RenderItems();
            }
        }

        protected TButtonType GetSlotItem()
        {
            return GetPool().Request(containerItem);
        }

        protected abstract void OnRenderItem();
    }
}