using System.Collections;
using UnityEngine;

namespace PoolingSystem.Samples
{
    public class Gun : MonoBehaviour
    {
        [SerializeField] 
        private PoolData<Bullet> data;

        [SerializeField] 
        private float shootInterval;
        [SerializeField] 
        private float bulletSpeed;
        
        private Pool<Bullet> pool;
        
        void Start()
        {
            pool = new Pool<Bullet>(data, transform);
            //Remplissage de la piscine
            pool.Initialize();
            
            //FEUUU
            StartCoroutine(IFire());
        }

        private IEnumerator IFire()
        {
            while (true)
            {
                yield return new WaitForSeconds(shootInterval);
                Bullet bullet = pool.Instantiate();
                if (bullet != null)
                {
                    bullet.transform.position = transform.position;
                    bullet.Fire(bulletSpeed);
                }
            }
        }
    }
}
