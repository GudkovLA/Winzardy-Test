#nullable enable

using System;
using Game.ResourceSystem.Settings;
using UnityEngine;

namespace Game.CharacterSystem.Settings
{
    [CreateAssetMenu(fileName = nameof(EnemySettings), menuName = "Assets/Enemy Settings")]
    [Serializable]
    public class EnemySettings : AbstractPlayerSettings, IDisposable
    {
        public LootSettingsData[] Loot;
        
        public void Dispose()
        {
        }

        [Serializable]
        public class LootSettingsData
        {
            public ResourceSettings Resource = null!;
            public int PoolSize;
            
            [Tooltip("Chance of coin will be dropped, where 1 means 100% of chance")]
            [Range(0, 1)]
            public float DropChance;
        }
    }
}