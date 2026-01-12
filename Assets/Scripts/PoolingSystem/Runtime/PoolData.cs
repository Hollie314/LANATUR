using UnityEngine;

namespace PoolingSystem
{
    public abstract class PoolData<TObject> : ScriptableObject 
        where TObject : Object
    {
        [field: SerializeField, Min(0)]
        public int PoolSize { get; private set; }
        
        [field: SerializeField]
        public TObject Prefab { get; private set; }
        
        [field: SerializeField]
        public PoolEmptyBehaviour EmptyBehaviour { get; private set; }
    }
}