#nullable enable

using System;
using System.Collections.Generic;
using Arch.Buffer;
using Arch.Core;
using Arch.Core.Extensions;
using Game.AbilitySystem.Components;
using Game.AbilitySystem.Settings;
using Game.Common;
using Game.Utils;
using UnityEngine;

namespace Game.AbilitySystem
{
    public class AbilityManager : IDisposable
    {
        private static readonly Dictionary<int, IAbility> _abilities = new();
        
        private readonly WorldHandle _worldHandle;
        private readonly InstancePool _instancePool;

        public AbilityManager(WorldHandle worldHandle, InstancePool instancePool)
        {
            _worldHandle = worldHandle;
            _instancePool = instancePool;
        }
        
        public void Dispose()
        {
            _abilities.Clear();
        }

        public void CreateAbilities(AbstractAbilitySettings[] abilitySettings, Entity abilityOwner)
        {
            foreach (var settings in abilitySettings)
            {
                CreateAbility(settings, abilityOwner);
            }
        }

        public void CreateAbility(AbstractAbilitySettings abilitySettings, Entity abilityOwner)
        {
            var abilityId = abilitySettings.GetInstanceID();
            if (!_abilities.TryGetValue(abilityId, out var ability))
            {
                ability = abilitySettings.Instantiate();
                ability.Prepare(_instancePool);
                
                _abilities.Add(abilityId, ability);
            }
            
            var abilityEntity = _worldHandle.Value.Create();
            abilityEntity.Add(new AbilityState
            {
                AbilityId = abilitySettings.GetInstanceID(),
                OwnerEntity = new EntityHandle(abilityOwner),
                ActivateTimeout = abilitySettings.ActivateTimeout
            });
        }

        public void ActivateAbility(int abilityId, CommandBuffer commandBuffer, Entity ownerEntity)
        {
            if (!_abilities.TryGetValue(abilityId, out var ability))
            {
                Debug.LogError($"Can't find ability (AbilityId={abilityId})");
                return;
            }
            
            ability.Activate(_worldHandle.Value, commandBuffer, ownerEntity);
        }

        public bool CanActivateAbility(int abilityId, Entity ownerEntity)
        {
            if (!_abilities.TryGetValue(abilityId, out var ability))
            {
                Debug.LogError($"Can't find ability (AbilityId={abilityId})");
                return false;
            }
            
            return ability.CanActivate(_worldHandle.Value, ownerEntity);
        }
    }
}