namespace UIs.Tablet
{
    public class UITabletFurniture : BaseUITabletItemShop<UITabletButtonSlotItem>
    {
        public ItemDataSO[] ItemDataSO { get; private set; }
        
        public void SetFurnitures(ItemDataSO[] itemData)
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
            }  
        }
    }
}