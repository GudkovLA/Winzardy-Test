#nullable enable

using System;
using Arch.Buffer;
using Arch.Core;
using Game.CharacterSystem.Components;
using Game.ResourceSystem.Components;
using UnityEngine;

namespace Game.CharacterSystem.Settings
{
    [CreateAssetMenu(fileName = nameof(PlayerSettings), menuName = "Assets/Player Settings")]
    [Serializable]
    public class PlayerSettings : CharacterSettings
    {
        public float CoinsCollectRadius;

        public override void Initialize(World world, CommandBuffer commandBuffer, Entity entity)
        {
            base.Initialize(world, commandBuffer, entity);
            
            commandBuffer.Add(entity, new ResourceCollector { CollectRadius = CoinsCollectRadius });
            commandBuffer.Add(entity, new PlayerTag());
        }
    }
}