using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace PoolingSystem
{
    public class Pool<T> : IPool where T : Object
    {
        public PoolData<T> Data { get; private set; }
        public int CurrentSize { get; private set; }
        public bool IsReady { get; private set; }
        public Transform Root { get; private set; }

        private Queue<T> poolQueue;
        private Queue<T> spawnedInstances;
        
        public Pool(PoolData<T> data, Transform root)
        {
            Root = root;
            Data = data;
            CurrentSize = -1;
            IsReady = false;
            poolQueue = new Queue<T>(Data.PoolSize);
            spawnedInstances = new Queue<T>(Data.PoolSize);
        }
        
        public void Initialize()
        {
            if(IsReady)
                return;
            
            CurrentSize = 0;
            
            ResizePool(Data.PoolSize);
            IsReady = true;
        }

        Object IPool.Instantiate()
        {
            return Instantiate();
        }
        void IPool.ReturnToPool(Object obj)
        {
            if(obj is T t)
                ReturnToPool(t);
        }
        
        public T Instantiate()
        {
            if (!IsReady)
                return null;
            
            if (poolQueue.TryDequeue(out T instance))
            {
                if(instance is IPooledObject pooledObject)
                    pooledObject.OnSpawn(this);

                spawnedInstances.Enqueue(instance);
                return instance;
            }

            switch (Data.EmptyBehaviour)
            {
                case PoolEmptyBehaviour.DontSpawn:
                    return null;
                case PoolEmptyBehaviour.AddOne:
                    ResizePool(CurrentSize + 1);
                    return Instantiate();
                case PoolEmptyBehaviour.MultiplyBy2:
                    ResizePool(CurrentSize * 2);
                    return Instantiate();
                case PoolEmptyBehaviour.NextPowerOfTwo:
                    int nextPow = Mathf.NextPowerOfTwo(CurrentSize);
                    ResizePool(nextPow);
                    return Instantiate();
                case PoolEmptyBehaviour.PickOldest:
                    if (spawnedInstances.TryPeek(out instance))
                    {
                        ReturnToPool(instance);
                        return Instantiate();
                    }
                    return null;
            }

            return null;
        }

        
        
        public void ReturnToPool(T instance)
        {
            if (!IsReady)
                return;

            if (instance is IPooledObject pooledObject)
                pooledObject.OnDespawn(this);
            
            poolQueue.Enqueue(instance);
            
            using (ListPool<T>.Get(out List<T> temp))
            {
                temp.AddRange(spawnedInstances);
                temp.Remove(instance);
                
                spawnedInstances.Clear();
                foreach (var t in temp)
                    spawnedInstances.Enqueue(t);
            }
        }
        
        private void ResizePool(int newSize)
        {
            int delta = newSize - CurrentSize;
            for (int i = 0; i < delta; i++)
            {
                T instance = Object.Instantiate(Data.Prefab, Root);
                
                if (instance is IPooledObject pooledObject)
                    pooledObject.OnCreated(this);
                
                poolQueue.Enqueue(instance);
            }

            CurrentSize = newSize;
        }
    }
}
