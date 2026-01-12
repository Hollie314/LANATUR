using System;
using UnityEngine;

namespace PoolingSystem.Samples
{
    public class Bullet : MonoBehaviour, IPooledObject
    {
        private Rigidbody2D rb2D;

        private IPool bulletPool;
        
        private void Awake()
        {
            rb2D = GetComponent<Rigidbody2D>();
        }

        public void Fire(float force)
        {
            rb2D.AddForce(Vector2.right * force, ForceMode2D.Impulse);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if(bulletPool != null)
                bulletPool.ReturnToPool(this);
            else
                Destroy(gameObject);
        }

        void IPooledObject.OnCreated(IPool pool)
        {
            bulletPool = pool;
            gameObject.SetActive(false);
        }

        void IPooledObject.OnDestroyed(IPool pool)
        {
            
        }

        void IPooledObject.OnSpawn(IPool pool)
        {
            gameObject.SetActive(true);
        }

        void IPooledObject.OnDespawn(IPool pool)
        {
            gameObject.SetActive(false);
        }
    }
}