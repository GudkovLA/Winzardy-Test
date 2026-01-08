#nullable enable

using System;
using Arch.Core;
using Game.CharacterSystem.Components;
using Game.Common;
using Game.PresentationSystem;
using Game.ResourceSystem.Settings;
using UnityEngine;

namespace Game.CharacterSystem.Settings
{
    [CreateAssetMenu(fileName = nameof(EnemySettings), menuName = "Assets/Enemy Settings")]
    [Serializable]
    public class EnemySettings : CharacterSettings
    {
        public float MinDistanceToPlayer;
        public LootSettingsData[] Loot = null!;
        
        public override void Prepare(InstancePool instancePool)
        {
            base.Prepare(instancePool);

            foreach (var lootSettingsData in Loot)
            {
                lootSettingsData.Resource.Prepare(instancePool);
            }
        }

        public override void Build(Entity entity, BuildContext context)
        {
            base.Build(entity, context);
            
            context.CommandBuffer.Add(entity, new EnemyControlState { MinDistanceToPlayer = MinDistanceToPlayer });
        }

        [Serializable]
        public class LootSettingsData
        {
            public ResourceSettings Resource = null!;
            
            [Tooltip("Chance of coin will be dropped, where 1 means 100% of chance")]
            [Range(0, 1)]
            public float DropChance;
        }
    }
}