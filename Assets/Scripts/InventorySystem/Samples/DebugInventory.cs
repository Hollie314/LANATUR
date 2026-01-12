using System;
using InventorySystem.Samples.UI;
using UnityEngine;

namespace InventorySystem.Samples
{
    public class DebugInventory : MonoBehaviour
    {
        [Serializable]
        private struct InventoryItemSetup
        {
            [field: SerializeField]
            public int Quantity { get; private set; }
            [field: SerializeField]
            public DefaultTradableItem Item { get; private set; }
        }

        [SerializeField] private InventoryItemSetup[] setups;
        [SerializeField] private TradableInventoryUI ui;

        private TradableInventory inventory;

        private void Awake()
        {
            inventory = new TradableInventory();
            for (int i = 0; i < setups.Length; i++)
            {
                InventoryItemSetup item = setups[i];
                inventory.AddOrRemoveItem(item.Item, item.Quantity);
            }
        }

        private void Start()
        {
            ui.Connect(inventory);
        }
    }
}
