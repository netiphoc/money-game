namespace UIs.Tablet
{
    public class UITabletProduct : BaseUITabletItemShop<UITabletButtonSlotItem>
    {
        public ItemDataSO[] ItemDataSO { get; private set; }
        
        public void SetProducts(ItemDataSO[] itemData)
        {
            ItemDataSO = itemData;
            RenderItems();
        }
        
        protected override void OnRenderItem()
        {
            foreach (var item in ItemDataSO)
            {
                UITabletButtonSlotItem uiTabletButtonSlotItem = GetSlotItem();
                uiTabletButtonSlotItem.SetItem(item);
                uiTabletButtonSlotItem.OnAddItemToCart = OnAddItemToCartEvent;
            }  
        }
    }
}