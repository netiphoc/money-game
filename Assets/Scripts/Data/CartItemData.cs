namespace Data
{
    public struct CartItemData
    {
        public ItemDataSO Item;
        public int Amount;

        public CartItemData(ItemDataSO item, int amount)
        {
            Item = item;
            Amount = amount;
        }
    }
}