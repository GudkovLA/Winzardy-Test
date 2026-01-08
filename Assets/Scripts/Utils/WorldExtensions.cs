#nullable enable

using Arch.Buffer;
using Arch.Core;
using Game.CharacterSystem.Components;
using Game.Common;
using Game.Common.Components;
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

            if (!serviceLocator.TryGet<GameSettings>(out var gameSettings)
                || !serviceLocator.TryGet<GameLevel>(out var gameLevel))
            {
                return;
            }

            var commandBuffer = new CommandBuffer();
            var entity = world.Create();
            gameSettings.Player.Build(entity, new BuildContext(world, commandBuffer));
            commandBuffer.Add(entity, new Position { Value = gameLevel.StartPosition });
            commandBuffer.Add(entity, new Rotation { Value = gameLevel.StartRotation });
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