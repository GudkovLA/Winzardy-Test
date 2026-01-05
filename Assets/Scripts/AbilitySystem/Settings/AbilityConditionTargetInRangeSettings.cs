#nullable enable

using System;
using Arch.Buffer;
using Arch.Core;
using Game.AbilitySystem.Components;
using UnityEngine;

namespace Game.AbilitySystem.Settings
{
    [CreateAssetMenu(fileName = nameof(AbilityConditionTargetInRangeSettings), 
        menuName = "Assets/Abilities/Ability Condition Target In Range")]
    [Serializable]
    public class AbilityConditionTargetInRangeSettings : AbstractAbilityConditionSettings
    {
        public float Distance;
        
        public override void Initialize(CommandBuffer commandBuffer, Entity entity)
        {
            base.Initialize(commandBuffer, entity);

            commandBuffer.Add(entity, new AbilityConditionTargetInRange
            {
                Distance = Distance
            });
        }
    }
}