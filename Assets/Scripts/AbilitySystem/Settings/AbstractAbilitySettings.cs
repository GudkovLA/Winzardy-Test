#nullable enable

using Arch.Core;
using Game.AbilitySystem.Components;
using Game.Common;
using Game.PresentationSystem;
using Game.PresentationSystem.Components;
using UnityEngine;

namespace Game.AbilitySystem.Settings
{
    public abstract class AbstractAbilitySettings : ScriptableObject, IPoolable, IEntityBuilder
    {
        public float CooldownDuration;
        
        public AbstractAbilityConditionSettings? ConditionSettings;

        public virtual void Prepare(InstancePool instancePool)
        {
        }

        public virtual void Build(Entity entity, BuildContext context)
        {
            context.CommandBuffer.Add(entity, new InvisibleTag());
            context.CommandBuffer.Add(entity, new AbilityCooldown { Duration = CooldownDuration });

            if (ConditionSettings != null)
            {
                ConditionSettings.Build(entity, context);
            }
        }
    }
}