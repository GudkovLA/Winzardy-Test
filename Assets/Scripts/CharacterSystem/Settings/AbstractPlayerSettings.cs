#nullable enable

using System;
using Arch.Buffer;
using Arch.Core;
using Game.AbilitySystem.Settings;
using Game.CharacterSystem.Components;
using UnityEngine;

namespace Game.CharacterSystem.Settings
{
    public abstract class AbstractPlayerSettings : ScriptableObject, IDisposable
    {
        public CharacterSettings Character;
        public int PoolSize;

        public FractionMask Fraction; 
        public FractionMask Enemies; 

        public AbstractAbilitySettings[] Abilities;

        public void Dispose()
        {
        }

        public virtual void Initialize(Entity entity, CommandBuffer commandBuffer)
        {
            Character.Initialize(entity, commandBuffer);
            
            commandBuffer.Add(entity, new Fraction
            {
                AlliesMask = Fraction,
                EnemiesMask = Enemies
            });
        }
    }
}