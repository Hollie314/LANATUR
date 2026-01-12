using UnityEngine;

namespace InventorySystem.UI
{
    public abstract class InventoryStackUI<T> : MonoBehaviour
        where T : IInventoryItem
    {
        private InventoryUI<T> currentInventoryUI;
        private int currentID;

        public InventoryStack<T> Stack => currentInventoryUI.Current.GetStack(currentID);

        public void Connect(InventoryUI<T> inventoryUI, int id)
        {
            if(currentInventoryUI != null)
                Disconnect();
            
            currentInventoryUI = inventoryUI;
            currentID = id;
            
            OnConnected();
        }
        public void Disconnect()
        {
            OnDisconnected();
            
            currentInventoryUI = null;
            currentID = -1;
        }

        protected abstract void OnConnected();
        protected abstract void OnDisconnected();
    }
}