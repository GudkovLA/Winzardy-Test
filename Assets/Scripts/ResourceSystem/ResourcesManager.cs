#nullable enable

using System;
using System.Collections.Generic;
using Game.CharacterSystem.Settings;
using Game.ResourceSystem.Settings;
using Game.Settings;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.ResourceSystem
{
    public class ResourcesManager : IDisposable
    {
        private static readonly Dictionary<int, ResourceData> _resources = new();
        private static readonly Dictionary<int, DropData> _drops = new();

        public void Dispose()
        {
            _resources.Clear();
        }

        public void CreateResources(GameSettings gameSettings)
        {
            for (var i = 0; i < gameSettings.Enemies.Length; i++)
            {
                var enemySettings = gameSettings.Enemies[i].EnemySettings;
                if (enemySettings == null)
                {
                    Debug.LogError($"Enemy settings is not defined");
                    continue;
                }

                CreateResources(enemySettings);
            }
        }

        public void CreateResources(EnemySettings enemySettings)
        {
            var enemyId = enemySettings.GetInstanceID();
            if (_drops.ContainsKey(enemyId))
            {
                return;
            }
            
            var drop = new DropData();
            foreach (var lootData in enemySettings.Loot)
            {
                if (lootData.Resource.Prefab == null)
                {
                    Debug.LogError($"Resource prefab is not defined");
                    continue;
                }

                var resourceId = lootData.Resource.Prefab.GetInstanceID();
                drop.ResourceIds.Add(resourceId);
                drop.ResourceChances.Add(lootData.DropChance);

                if (!_resources.ContainsKey(resourceId))
                {
                    _resources.Add(resourceId, new ResourceData(lootData.Resource));
                }
            }
            
            _drops.Add(enemyId, drop);
        }

        public void CollectResource(int resourceId, int amount = 1)
        {
            if (!_resources.TryGetValue(resourceId, out var resourceData))
            {
                Debug.LogError($"Can't find resource (ResourceId={resourceId})");
                return;
            }

            resourceData.Amount += amount;
        }

        public bool TryGetDroppedResource(int enemyId, out int resourceId)
        {
            if (!_drops.TryGetValue(enemyId, out var drop))
            {
                Debug.LogError($"Can't find drop settings (EnemyId={enemyId})");
                resourceId = 0;
                return false;
            }

            var bestIndex = -1;
            var bestValue = 0f;
            var dropValue = Random.value;
            for (var i = 0; i < drop.ResourceChances.Count; i++)
            {
                if (dropValue > drop.ResourceChances[i])
                {
                    continue;
                }

                if (drop.ResourceChances[i] < bestValue
                    || bestIndex == -1)
                {
                    bestIndex = i;
                    bestValue = dropValue;
                    continue;
                }

                if (Random.value < 0.5f)
                {
                    bestIndex = i;
                }
            }

            if (bestIndex == -1)
            {
                resourceId = 0;
                return false;
            }

            resourceId = drop.ResourceIds[bestIndex];
            return true;
        }

        public ResourceSettings? GetResource(int resourceId)
        {
            if (!_resources.TryGetValue(resourceId, out var resourceData))
            {
                Debug.LogError($"Can't find resource (ResourceId={resourceId})");
                return null;
            }

            return resourceData.Settings;
        }

        public IEnumerable<ResourceData> GetResources()
        {
            return _resources.Values;
        }

        public class ResourceData
        {
            public ResourceSettings Settings;
            public int Amount;

            public ResourceData(ResourceSettings settings, int amount = 0)
            {
                Settings = settings;
                Amount = amount;
            }
        }

        public class DropData
        {
            public readonly List<int> ResourceIds = new();
            public readonly List<float> ResourceChances = new();
        }
    }
}