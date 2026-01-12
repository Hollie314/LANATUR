using UnityEngine;

namespace PoolingSystem
{
    public interface IPool
    {
        int CurrentSize { get; }
        bool IsReady { get; }
        Transform Root { get; }

        Object Instantiate();
        void ReturnToPool(Object obj);
    }
}