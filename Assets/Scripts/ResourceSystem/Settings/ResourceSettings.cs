#nullable enable

using System;
using Game.Settings;
using Game.Utils;
using UnityEngine;

namespace Game.ResourceSystem.Settings
{
    [CreateAssetMenu(fileName = nameof(ResourceSettings), menuName = "Assets/Resource Settings")]
    [Serializable]
    public class ResourceSettings : ScriptableObject, IDisposable, IPoolable
    {
        public GameObject? Prefab;
        public int PoolSize;
        
        public Sprite? Icon;

        public void Dispose()
        {
        }

        public void Prepare(InstancePool instancePool)
        {
            if (Prefab != null)
            {
                instancePool.Register(Prefab, PoolSize);
            }
        }
    }
}