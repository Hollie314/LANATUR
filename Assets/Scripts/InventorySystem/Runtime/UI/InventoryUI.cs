using System;
using System.Collections.Generic;
using UnityEngine;

namespace InventorySystem.UI
{
    public abstract class InventoryUI<T> : MonoBehaviour 
        where T : IInventoryItem
    {
        [SerializeField]
        private Transform container;
        [SerializeField]
        private InventoryStackUI<T> itemPrefab;
        public Inventory<T> Current { get; private set; }

        private List<InventoryStackUI<T>> spawnedInventoryStack;

        private void Awake()
        {
            spawnedInventoryStack = new();
            ClearItems();
        }

        public void Connect(Inventory<T> inventory)
        {
            if(Current != null)
                Disconnect();

            Current = inventory;
            Current.OnItemUpdated += RefreshUI;

            SpawnItems(inventory);
            OnConnected();
        }

        public void Disconnect()
        {
            OnDisconnected();
            ClearItems();

            Current.OnItemUpdated -= RefreshUI;
            Current = null;
        }

        private void ClearItems()
        {
            foreach (InventoryStackUI<T> stackUI in spawnedInventoryStack)
                stackUI.Disconnect();

            foreach (Transform child in container)
                Destroy(child.gameObject);
        }

        private void SpawnItems(Inventory<T> inventory)
        {
            int index = 0;
            foreach (InventoryStack<T> stack in inventory.Stacks)
            {
                //Spawn une UI en enfant de container
                InventoryStackUI<T> instance = Instantiate(itemPrefab, container);
                
                //Syncro des infos
                instance.Connect(this, index);
                
                //On l'ajoute a notre liste d'UI spawnées
                spawnedInventoryStack.Add(instance);
                index++;
            }
        }


        private void RefreshUI(T item)
        {
            //Horrible m'enfin
            ClearItems();
            SpawnItems(Current);
        }

        protected abstract void OnConnected();
        protected abstract void OnDisconnected();
    }
}