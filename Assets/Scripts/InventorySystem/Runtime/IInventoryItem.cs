using UnityEngine;

namespace InventorySystem
{
    public interface IInventoryItem
    {
        string ID { get; }
        
        string Name { get; }
        
        string Description { get; }
        
        int StackableQuantity { get; }
    }
}
