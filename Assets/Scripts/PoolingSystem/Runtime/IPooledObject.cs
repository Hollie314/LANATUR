namespace PoolingSystem
{
    public interface IPooledObject
    {
        void OnCreated(IPool pool);
        void OnDestroyed(IPool pool);
        
        void OnSpawn(IPool pool);
        void OnDespawn(IPool pool);
    }
}