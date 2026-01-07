#nullable enable

using Arch.Buffer;
using Arch.Core;
using Game.AbilitySystem.Components;
using UnityEngine;

namespace Game.AbilitySystem.Settings
{
    public abstract class AbstractAbilityConditionSettings : ScriptableObject
    {
        public virtual void Initialize(CommandBuffer commandBuffer, Entity entity)
        {
            commandBuffer.Add(entity, new AbilityBlockedTag());
        }
    }
}