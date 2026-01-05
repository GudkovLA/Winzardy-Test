#nullable enable

using System;
using Arch.Buffer;
using Arch.Core;
using Game.CharacterSystem.Components;
using Game.ResourceSystem.Settings;
using UnityEngine;

namespace Game.CharacterSystem.Settings
{
    [CreateAssetMenu(fileName = nameof(EnemySettings), menuName = "Assets/Enemy Settings")]
    [Serializable]
    public class EnemySettings : AbstractPlayerSettings, IDisposable
    {
        public float MinDistanceToPlayer;

        public LootSettingsData[] Loot;
        
        public void Dispose()
        {
        }
        
        public override void Initialize(World world, CommandBuffer commandBuffer, Entity entity)
        {
            base.Initialize(world, commandBuffer, entity);
            
            commandBuffer.Add(entity, new EnemyControlState { MinDistanceToPlayer = MinDistanceToPlayer });
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