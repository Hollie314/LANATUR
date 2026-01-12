using System;
using UnityEngine;

namespace InventorySystem.Samples
{
    [CreateAssetMenu(menuName = "Inventory/Samples/Tradable", order = 0)]
    public class DefaultTradableItem : ScriptableObject, ITradableItem
    {
        [field: SerializeField, HideInInspector]
        public string ID { get; private set; }
        
        [field: SerializeField]
        public string Name { get; private set;}
        
        [field: SerializeField]
        public string Description { get; private set;}
        
        [field: SerializeField]
        public int StackableQuantity { get; private set;}
        
        [field: SerializeField]
        public Sprite Icon { get; private set;}
        
        [field: SerializeField]
        public int Price { get; private set; }

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(ID))
            {
                ID = Guid.NewGuid().ToString();
            }
        }

        private void Reset()
        {
            ID = null;
            OnValidate();
        }
    }
}