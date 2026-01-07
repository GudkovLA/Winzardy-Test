#nullable enable

using Arch.Buffer;
using Arch.Core;
using Game.AbilitySystem.Components;
using Game.PresentationSystem;
using Game.PresentationSystem.Components;
using UnityEngine;

namespace Game.AbilitySystem.Settings
{
    public abstract class AbstractAbilitySettings : ScriptableObject, IPoolable
    {
        public float CooldownDuration;
        
        public AbstractAbilityConditionSettings? ConditionSettings;

        public virtual void Prepare(InstancePool instancePool)
        {
        }

        public virtual void Initialize(CommandBuffer commandBuffer, Entity entity)
        {
            commandBuffer.Add(entity, new InvisibleTag());
            commandBuffer.Add(entity, new AbilityCooldown { Duration = CooldownDuration });

            if (ConditionSettings != null)
            {
                ConditionSettings.Initialize(commandBuffer, entity);
            }
        }
    }
}