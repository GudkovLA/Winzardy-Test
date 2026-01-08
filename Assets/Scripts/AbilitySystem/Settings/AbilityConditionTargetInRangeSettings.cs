#nullable enable

using System;
using Arch.Core;
using Game.AbilitySystem.Components;
using Game.Common;
using UnityEngine;

namespace Game.AbilitySystem.Settings
{
    [CreateAssetMenu(fileName = nameof(AbilityConditionTargetInRangeSettings), 
        menuName = "Assets/Abilities/Ability Condition Target In Range")]
    [Serializable]
    public class AbilityConditionTargetInRangeSettings : AbstractAbilityConditionSettings
    {
        public float Distance;
        
        public override void Build(Entity entity, BuildContext context)
        {
            base.Build(entity, context);

            context.CommandBuffer.Add(entity, new AbilityConditionTargetInRange
            {
                Distance = Distance
            });
        }
    }
}