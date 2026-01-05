#nullable enable

using System;
using Arch.Buffer;
using Arch.Core;
using Game.AbilitySystem.Components;
using Game.AbilitySystem.Settings;
using Game.CharacterSystem.Components;
using Game.Common;
using UnityEngine;

namespace Game.CharacterSystem.Settings
{
    public abstract class AbstractPlayerSettings : ScriptableObject, IDisposable
    {
        public CharacterSettings Character = null!;
        public int PoolSize;

        public FractionMask Fraction; 
        public FractionMask Enemies; 

        public AbstractAbilitySettings[] Abilities = null!;

        public void Dispose()
        {
        }

        public virtual void Initialize(World world, CommandBuffer commandBuffer, Entity entity)
        {
            Character.Initialize(entity, commandBuffer);
            
            commandBuffer.Add(entity, new Fraction
            {
                AlliesMask = Fraction,
                EnemiesMask = Enemies
            });
            
            foreach (var abilitySettings in Abilities)
            {
                var abilityEntity = world.Create();
                commandBuffer.Add(abilityEntity, new Ability
                {
                    OwnerEntity = new EntityHandle(entity),
                    LastActivateTime = Time.realtimeSinceStartup
                });
                
                abilitySettings.Initialize(commandBuffer, abilityEntity);
            }
        }
    }
}