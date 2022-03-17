using System;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class PoolingManager : MonoBehaviour
    {
        #region Singleton

        public static PoolingManager Instance;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        #endregion

        [SerializeField] private GameObject poolParent;
        [SerializeField] private Queue<GameObject> PoolableObjectList = new Queue<GameObject>();
        [SerializeField] private GameObject poolPrefab;
        [SerializeField] private int poolAmount = 100;

        public void Start()
        {
            Setup();
        }

        private void Setup()
        {
            PoolableObjectList = new Queue<GameObject>();

            for (int i = 0; i < poolAmount; i++)
            {
                var go = Instantiate(poolPrefab, poolParent.transform, true);
                go.SetActive(false);

                PoolableObjectList.Enqueue(go);
            }
        }

        public void EnqueueBullet(GameObject poolObject)
        {
            poolObject.transform.parent = poolParent.transform;
            poolObject.transform.localPosition = Vector3.zero;
            poolObject.transform.localEulerAngles = Vector3.zero;

            poolObject.gameObject.SetActive(false);

            PoolableObjectList.Enqueue(poolObject);
        }

        public GameObject DequeuePoolableGameObject()
        {
            var deQueuedPoolObject = PoolableObjectList.Dequeue();
            if (deQueuedPoolObject.activeSelf) DequeuePoolableGameObject();
            deQueuedPoolObject.SetActive(true);
            return deQueuedPoolObject;
        }
    }
}