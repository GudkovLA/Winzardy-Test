#nullable enable

using Arch.Core;
using Game.AbilitySystem.Components;
using Game.Common;
using UnityEngine;

namespace Game.AbilitySystem.Settings
{
    public abstract class AbstractAbilityConditionSettings : ScriptableObject, IEntityBuilder
    {
        public virtual void Build(Entity entity, BuildContext context)
        {
            context.CommandBuffer.Add(entity, new AbilityBlockedTag());
        }
    }
}