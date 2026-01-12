using UnityEngine;

namespace InventorySystem.Samples
{
    public interface ITradableItem : IInventoryItem
    {
        Sprite Icon { get; }
        int Price { get; }
    }
}