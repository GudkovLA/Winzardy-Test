#nullable enable

using Arch.Core;
using Game.CharacterSystem.Components;
using Game.CharacterSystem.Settings;
using Game.Common;
using Game.Common.Components;
using Game.Components;
using Game.DamageSystem.Components;
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
                || !serviceLocator.TryGet<GameLevel>(out var gameLevel))
            {
                return;
            }

            if (playerSettings.Character.Prefab == null)
            {
                Debug.LogError($"Character prefab is not defined");
                return;
            }

            var entity = world.Create();
            entity.Add(entity, new Position { Value = gameLevel.StartPosition });
            entity.Add(entity, new Rotation { Value = gameLevel.StartRotation });
            entity.Add(entity, new Size { Value = playerSettings.Character.Size });
            entity.Add(entity, new PrefabId { Value = playerSettings.Character.Prefab.GetInstanceID() });
            entity.Add(entity, new HealthState
            {
                MaxHealth = playerSettings.Character.MaxHealth,
                Health = playerSettings.Character.MaxHealth
            });
            entity.Add(entity, new CoinCollector
            {
                CollectRadius = playerSettings.CoinsCollectRadius
            });
            entity.Add(entity, new PlayerTag());
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