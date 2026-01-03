#nullable enable

using Arch.Buffer;
using Arch.Core;
using Game.AbilitySystem;
using Game.CharacterSystem;
using Game.CharacterSystem.Components;
using Game.CharacterSystem.Settings;
using Game.Common;
using UnityEngine;

namespace Game.Utils
{
    public static class WorldExtensions
    {
        public static void CreatePlayerSingleton(this World world, ServiceLocator serviceLocator)
        {
            var entitiesCount = world.CountEntities(new QueryDescription().WithAll<PlayerTag>());
            if (entitiesCount == 1)
            {
                Debug.LogWarning($"Player already created");
                return;
            }

            if (entitiesCount > 1)
            {
                Debug.LogError($"Only one player possible");
                return;
            }

            if (!serviceLocator.TryGet<PlayerSettings>(out var playerSettings)
                || !serviceLocator.TryGet<GameLevel>(out var gameLevel)
                || !serviceLocator.TryGet<AbilityManager>(out var abilityManager))
            {
                return;
            }

            var commandBuffer = new CommandBuffer();
            var entity = CharacterUtils.SpawnCharacter(world, 
                playerSettings,
                abilityManager,
                commandBuffer,
                gameLevel.StartPosition,
                gameLevel.StartRotation);

            if (entity == Entity.Null)
            {
                return;
            }
            
            commandBuffer.Playback(world);
        }

        public static Entity GetPlayerSingleton(this World world)
        {
            var count = 0;
            var result = Entity.Null;

            world.Query(new QueryDescription().WithAll<PlayerTag>(), entity=>
            {
                result = entity;
                count++;
            });
            
            if (count == 0)
            {
                Debug.LogError($"Player is not created");
                return Entity.Null;
            }

            if (count > 1)
            {
                Debug.LogError($"Only one player possible");
            }

            return result;
        }
    }
}