#nullable enable

using System;
using Arch.Core;
using Game.CharacterSystem.Components;
using Game.Common;
using Game.ResourceSystem.Components;
using UnityEngine;

namespace Game.CharacterSystem.Settings
{
    [CreateAssetMenu(fileName = nameof(PlayerSettings), menuName = "Assets/Player Settings")]
    [Serializable]
    public class PlayerSettings : CharacterSettings
    {
        public float CoinsCollectRadius;

        public override void Build(Entity entity, BuildContext context)
        {
            base.Build(entity, context);
            
            context.CommandBuffer.Add(entity, new ResourceCollector { CollectRadius = CoinsCollectRadius });
            context.CommandBuffer.Add(entity, new PlayerTag());
        }
    }
}