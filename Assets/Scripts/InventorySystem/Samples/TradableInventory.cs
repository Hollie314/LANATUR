using System;

namespace InventorySystem.Samples
{
    public class TradableInventory : Inventory<ITradableItem>
    {
        public event Action<int> OnMoneyChanged; 
        public int Money { get; private set; }

        
        public void Sell(ITradableItem item, int quantity)
        {
            AddOrRemoveItem(item, -quantity);
            Money += item.Price * quantity;
            OnMoneyChanged?.Invoke(Money);
        }

        public void Buy(ITradableItem item, int quantity)
        {
            AddOrRemoveItem(item, quantity);
            Money -= item.Price * quantity;
            OnMoneyChanged?.Invoke(Money);
        }
    }
}