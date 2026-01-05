#nullable enable

using System;
using Arch.Buffer;
using Arch.Core;
using Game.CharacterSystem.Components;
using Game.DamageSystem.Components;
using Game.LocomotionSystem.Components;
using Game.ResourceSystem.Components;
using UnityEngine;

namespace Game.CharacterSystem.Settings
{
    [CreateAssetMenu(fileName = nameof(PlayerSettings), menuName = "Assets/Player Settings")]
    [Serializable]
    public class PlayerSettings : AbstractPlayerSettings, IDisposable
    {
        public float CoinsCollectRadius;

        public override void Initialize(World world, CommandBuffer commandBuffer, Entity entity)
        {
            base.Initialize(world, commandBuffer, entity);
            
            commandBuffer.Add(entity, new ResourceCollector { CollectRadius = CoinsCollectRadius });
            commandBuffer.Add(entity, new PlayerTag());
            commandBuffer.Add(entity, new IgnoreObstaclesTag());
            commandBuffer.Add(entity, new DontDestroyOnDeath());
        }
    }
}