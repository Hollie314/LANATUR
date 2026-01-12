using System;
using System.Collections.Generic;
using UnityEngine;

namespace InventorySystem
{
    public class Inventory<T> where T : IInventoryItem
    {
        public event Action<T> OnItemUpdated; 
        public event Action<T, int> OnItemWasAdded;
        public event Action<T, int> OnItemWasRemoved;
        
        private readonly List<InventoryStack<T>> stacks;
        
        public IReadOnlyList<InventoryStack<T>> Stacks 
            => stacks.AsReadOnly();

        public Inventory()
        {
            stacks = new List<InventoryStack<T>>();
        }


        public void AddOrRemoveItem(T item, int quantity)
        {
            //Si on ajoute
            if (quantity > 0)
            {
                AddItem(item, quantity);
                /*
                if (OnItemWasAdded != null)
                    OnItemWasAdded(item, quantity);
                */
                //Meme chose
                OnItemWasAdded?.Invoke(item, quantity);
            }
            else if (quantity < 0)
            {
                RemoveItem(item, quantity);
                OnItemWasRemoved?.Invoke(item, -quantity);
            }
            
            OnItemUpdated?.Invoke(item);
        }

        private void AddItem(T item, int quantity)
        {
            int quantityToAdd = quantity;
            //On complete les piles deja existantes
            for (var i = 0; i < stacks.Count; i++)
            {
                InventoryStack<T> stack = stacks[i];
                if (stack.item.ID != item.ID)
                    continue;
                //Taille max d'une stack
                int stackableQuantity = stack.item.StackableQuantity;
                    
                //Si -1 taille infinie, sinon taille max - taille actuelle
                int canAddInStack = stackableQuantity < 0 ? 
                    int.MaxValue :
                    stackableQuantity - stack.quantity;

                int quantityToAddInStack = Mathf.Min(quantityToAdd, canAddInStack);
                if (quantityToAddInStack <= 0)
                    continue;

                quantityToAdd -= quantityToAddInStack;
                    
                int newQuantity = stack.quantity + quantityToAddInStack;
                InventoryStack<T> newStack = new InventoryStack<T>(item, newQuantity);
                stacks[i] = newStack;
            }

            if (item.StackableQuantity < 0)
            {
                InventoryStack<T> remainingStack = new(item, quantityToAdd);
                stacks.Add(remainingStack);
            }
            else
            {
                int fullStackCount = Mathf.FloorToInt((float)quantityToAdd / item.StackableQuantity);
                int remaining = quantityToAdd % item.StackableQuantity;
                for (int i = 0; i < fullStackCount; i++)
                {
                    InventoryStack<T> remainingStack = new(item, item.StackableQuantity);
                    stacks.Add(remainingStack);   
                }

                if (remaining > 0)
                {
                    InventoryStack<T> remainingStack = new(item, remaining);
                    stacks.Add(remainingStack);   
                }
            }
        }
        private void RemoveItem(T item, int quantity)
        {
            int quantityToRemove = -quantity;
            for (int i = 0; i < stacks.Count; i++)
            {
                InventoryStack<T> stack = stacks[i];
                if (stack.item.ID != item.ID)
                    continue;

                int toRemove = Mathf.Min(stack.quantity, quantityToRemove);
                if (toRemove > 0)
                {
                    quantityToRemove -= toRemove;
                        
                    InventoryStack<T> newStack = new InventoryStack<T>(item, stack.quantity - toRemove);
                    stacks[i] = newStack;
                }
                    
                if(quantityToRemove <= 0)
                    break;
            }
                
            //Trust
            stacks.RemoveAll(ctx => ctx.quantity == 0);
        }
        
        public int GetItemQuantity(T item)
        {
            int count = 0;
            foreach (InventoryStack<T> stack in stacks)
            {
                if (stack.item.ID == item.ID)
                    count += stack.quantity;
            }

            return count;
        }

        public InventoryStack<T> GetStack(int index) => stacks[index];
    }
}