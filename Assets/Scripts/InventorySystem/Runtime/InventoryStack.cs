namespace InventorySystem
{
    public struct InventoryStack<T> where T : IInventoryItem
    {
        public readonly int quantity;
        public readonly T item;

        public InventoryStack(T item, int quantity = 1)
        {
            this.quantity = quantity;
            this.item = item;
        }
    }
}