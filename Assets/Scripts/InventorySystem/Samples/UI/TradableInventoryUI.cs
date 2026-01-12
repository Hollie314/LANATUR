using InventorySystem.UI;
using TMPro;
using UnityEngine;

namespace InventorySystem.Samples.UI
{
    public class TradableInventoryUI : InventoryUI<ITradableItem>
    {
        [SerializeField] 
        private TMP_Text moneyText;
        
        protected override void OnConnected()
        {
            if (Current is TradableInventory tradableInventory)
            {
                tradableInventory.OnMoneyChanged += OnMoneyChanged;
                OnMoneyChanged(tradableInventory.Money);
            }
        }

        protected override void OnDisconnected()
        {
            if (Current is TradableInventory tradableInventory)
            {
                tradableInventory.OnMoneyChanged -= OnMoneyChanged;
            }
        }
        
        private void OnMoneyChanged(int value)
        {
            moneyText.text = value.ToString();
        }
    }
}