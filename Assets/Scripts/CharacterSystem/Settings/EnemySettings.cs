#nullable enable

using System;
using Arch.Core;
using Game.CharacterSystem.Components;
using Game.Common;
using Game.PresentationSystem;
using Game.ResourceSystem.Settings;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.CharacterSystem.Settings
{
    [CreateAssetMenu(fileName = nameof(EnemySettings), menuName = "Assets/Enemy Settings")]
    [Serializable]
    public class EnemySettings : CharacterSettings
    {
        private readonly int[] _randomDirectionFactor = { -1, 1 };

        public float MinDistanceToPlayer;
        public float ObstacleAvoidanceDistance;
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
            
            var directionIndex = Random.Range(0, _randomDirectionFactor.Length);
            context.CommandBuffer.Add(entity, new EnemyControlState
            {
                ObstacleAvoidanceDirection = _randomDirectionFactor[directionIndex],
                ObstacleAvoidanceDistance = ObstacleAvoidanceDistance,
                MinDistanceToPlayer = MinDistanceToPlayer
            });
        }

        [Serializable]
        public class LootSettingsData
        {
            public ResourceSettings Resource = null!;
            
            [Tooltip("Chance of resource will be dropped, where 1 means 100% of chance")]
            [Range(0, 1)]
            public float DropChance;
        }
    }
}