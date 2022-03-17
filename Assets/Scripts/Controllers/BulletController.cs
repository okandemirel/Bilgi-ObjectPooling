using Managers;
using UnityEngine;

namespace Controllers
{
    public class BulletController : MonoBehaviour
    {
        public bool IsCalledByPooling;

        private void OnEnable()
        {
            Invoke(nameof(EnqueueCheck), 1);
        }

        private void EnqueueCheck()
        {
            if (IsCalledByPooling)
            {
                Invoke(nameof(Enqueue), 1);
            }
        }

        private void Enqueue()
        {
            IsCalledByPooling = false;
            PoolingManager.Instance.EnqueueBullet(gameObject);
        }
    }
}