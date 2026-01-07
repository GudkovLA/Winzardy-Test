#nullable enable

using Arch.Buffer;
using Arch.Core;
using Game.CharacterSystem.Settings;
using Game.Common.Components;
using UnityEngine;

namespace Game.CharacterSystem
{
    public static class CharacterUtils
    {
        public static Entity SpawnCharacter(
            CharacterSettings characterSettings,
            World world,
            CommandBuffer commandBuffer,
            Vector3 position,
            Quaternion rotation)
        {
            var entity = world.Create();
            commandBuffer.Add(entity, new Position { Value = position });
            commandBuffer.Add(entity, new Rotation { Value = rotation });
            
            characterSettings.Initialize(world, commandBuffer, entity);
            return entity;
        }
    }
}