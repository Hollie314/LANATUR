using InventorySystem.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InventorySystem.Samples.UI
{
    public class TradableStackUI : InventoryStackUI<ITradableItem>
    {
        [SerializeField] private TMP_Text title;
        [SerializeField] private TMP_Text quantity;
        [SerializeField] private TMP_Text price;
        [SerializeField] private Image icon;
        
        protected override void OnConnected()
        {
            quantity.text = Stack.quantity.ToString();
            
            title.text = Stack.item.Name;
            price.text = Stack.item.Price.ToString();
            icon.sprite = Stack.item.Icon;
        }
        
        protected override void OnDisconnected() { }
    }
}