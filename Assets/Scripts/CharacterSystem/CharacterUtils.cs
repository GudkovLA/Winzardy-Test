#nullable enable

using Arch.Buffer;
using Arch.Core;
using Game.AbilitySystem;
using Game.CharacterSystem.Settings;
using Game.Common.Components;
using UnityEngine;

namespace Game.CharacterSystem
{
    public static class CharacterUtils
    {
        public static Entity SpawnCharacter(
            World world, 
            AbstractPlayerSettings playerSettings,
            AbilityManager abilityManager,
            CommandBuffer commandBuffer,
            Vector3 position,
            Quaternion rotation)
        {
            var entity = world.Create();
            commandBuffer.Add(entity, new Position { Value = position });
            commandBuffer.Add(entity, new Rotation { Value = rotation });
            
            playerSettings.Initialize(entity, commandBuffer);
            abilityManager.CreateAbilities(playerSettings.Abilities, entity);

            return entity;
        }
    }
}