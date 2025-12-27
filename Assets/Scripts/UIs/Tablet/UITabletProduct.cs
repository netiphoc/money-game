using UnityEngine;

namespace UIs.Tablet
{
    public class UITabletProduct : BaseUI
    {
        [Header("Product")]
        [SerializeField] private UITabletButtonSlotItem uiTabletButtonSlotItemPrefab;
        [SerializeField] private Transform containerProduct;
        public ItemDataSO[] ItemDataSO { get; private set; }
        
        private PoolingUI<UITabletButtonSlotItem> _poolingUI;

        private PoolingUI<UITabletButtonSlotItem> GetPool()
        {
            return _poolingUI ??= new PoolingUI<UITabletButtonSlotItem>(uiTabletButtonSlotItemPrefab);
        }
        
        public void SetProducts(ItemDataSO[] itemData)
        {
            ItemDataSO = itemData;

            RenderProduct();
        }
        
        public void RenderProduct()
        {
            if(!isActiveAndEnabled) return;
            Debug.Log($"RenderProduct: {name}");
            GetPool().ClearPool();
            
            foreach (var item in ItemDataSO)
            {
                UITabletButtonSlotItem uiTabletButtonSlotItem = GetPool().Request(containerProduct);
                uiTabletButtonSlotItem.SetItem(item);
            }
        }

        public override void SetVisible(bool visible)
        {
            base.SetVisible(visible);

            if (visible)
            {
                RenderProduct();
            }
        }
    }
}