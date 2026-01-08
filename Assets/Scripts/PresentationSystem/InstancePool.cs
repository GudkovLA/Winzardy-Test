#nullable enable

using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.PresentationSystem
{
    public class InstancePool : IDisposable
    {
        private readonly Dictionary<int, Entry> _pools = new();
        private readonly Transform _root;

        public InstancePool(Transform root)
        {
            _root = root;
        }

        public void Dispose()
        {
            foreach (var entry in _pools.Values)
            {
                entry.Dispose();
            }
            
            _pools.Clear();
        }
        
        public void Register(GameObject prefab, int instanceCount = 0)
        {
            var prefabId = prefab.GetInstanceID();
            if (_pools.ContainsKey(prefabId))
            {
                return;
            }

            var entry = new Entry(prefab, _root);
            entry.Prepare(instanceCount);
            _pools.Add(prefabId, entry);
        }

        public GameObject? Get(int prefabId)
        {
            if (!_pools.TryGetValue(prefabId, out var entry))
            {
                Debug.LogError($"Pool is not registered (PrefabId={prefabId})");
                return null;
            }

            return entry.Get();
        }

        public void Release(int prefabId, GameObject instance)
        {
            if (!_pools.TryGetValue(prefabId, out var entry))
            {
                Debug.LogError($"Pool is not registered (PrefabId={prefabId})");
                Object.Destroy(instance);
                return;
            }

            if (_root == null)
            {
                // Application has been closed
                Object.Destroy(instance);
                return;
            }
            
            instance.transform.SetParent(_root);
            instance.SetActive(false);
            entry.Release(instance);
        }

        private class Entry : IDisposable
        {
            private readonly List<GameObject> _instances = new();
            private readonly GameObject _prefab;
            private readonly Transform _root;

            public Entry(GameObject prefab, Transform root)
            {
                _prefab = prefab;
                _root = root;
            }

            public void Dispose()
            {
                foreach (var gameObject in _instances)
                {
                    Object.Destroy(gameObject);
                }
                
                _instances.Clear();
            }

            public void Prepare(int instanceCount)
            {
                while (_instances.Count < instanceCount)
                {
                    var instance = Object.Instantiate(_prefab, _root);
                    instance.SetActive(false);
                    _instances.Add(instance);
                }
            }

            public GameObject Get()
            {
                if (_instances.Count > 0)
                {
                    var result = _instances[0];
                    _instances.RemoveAt(0);
                    return result;
                }

                Debug.LogWarning($"Not enough pool size (PrefabId={_prefab.GetInstanceID()};PrefabName={_prefab.name})");
                return Object.Instantiate(_prefab);
            }

            public void Release(GameObject instance)
            {
                _instances.Add(instance);
            }
        }
    }
}