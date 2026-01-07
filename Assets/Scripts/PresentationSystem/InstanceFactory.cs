#nullable enable

using System;
using UnityEngine;

namespace Game.PresentationSystem
{
    public class InstanceFactory : IDisposable
    {
        private readonly InstancePool _instancePool;

        public InstanceFactory(InstancePool instancePool)
        {
            _instancePool =  instancePool;
        }

        public void Dispose()
        {
            _instancePool.Dispose();
        }

        public GameObject? Create(int prefabId, Transform parent)
        {
            var instance = _instancePool.Get(prefabId);
            if (instance == null)
            {
                return null;
            }
            
            instance.transform.SetParent(parent);
            instance.SetActive(true);
            return instance;
        }

        public GameObject? Create(int prefabId, Transform parent, Vector3 position, Quaternion rotation)
        {
            var instance = _instancePool.Get(prefabId);
            if (instance == null)
            {
                return null;
            }

            var transform = instance.transform;
            transform.SetParent(parent);
            transform.position = position;
            transform.rotation = rotation;
            instance.SetActive(true);
            return instance;
        }

        public void Destroy(int prefabId, GameObject instance)
        {
            _instancePool.Release(prefabId, instance);
        }
    }
}