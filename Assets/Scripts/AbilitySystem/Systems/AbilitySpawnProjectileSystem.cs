#nullable enable

using Arch.Core;
using Arch.Core.Extensions;
using Game.AbilitySystem.Components;
using Game.CharacterSystem.Components;
using Game.Common.Components;
using Game.Common.Systems;
using Game.Common.Systems.Attributes;
using Game.DamageSystem.Components;
using Game.LocomotionSystem.Components;
using Game.PresentationSystem.Components;
using Game.ProjectileSystem.Components;
using Game.Utils;
using UnityEngine;

namespace Game.AbilitySystem.Systems
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class AbilitySpawnProjectileSystem : AbstractSystem
    {
        private static readonly QueryDescription _abilitiesQuery = new QueryDescription()
            .WithAll<AbilitySpawnProjectile, Ability, ProjectileData, LocomotionData, AbilityReadyTag>()
            .WithNone<Destroy, AbilityBlockedTag>();

        protected override void OnUpdate()
        {
            var world = Context.World;
            var commandBuffer = Context.GetOrCreateCommandBuffer(this);
            
            World.Query(_abilitiesQuery,
                (Entity entity, 
                    ref Ability ability, 
                    ref AbilitySpawnProjectile spawnAbility, 
                    ref ProjectileData projectileData,
                    ref LocomotionData locomotionData) =>
                {
                    if (!ability.OwnerEntity.IsValid())
                    {
                        commandBuffer.Destroy(entity);
                        return;
                    }

                    var ownerEntity = ability.OwnerEntity.Value;
                    if (!ownerEntity.TryGet<Position>(out var position))
                    {
                        Debug.LogError($"Can't find required component in ability owner (ComponentType={nameof(Position)})");
                        return;
                    }
            
                    if (!ownerEntity.TryGet<Rotation>(out var rotation))
                    {
                        Debug.LogError($"Can't find required component in ability owner (ComponentType={nameof(Rotation)})");
                        return;
                    }

                    if (!ownerEntity.TryGet<Size>(out var size))
                    {
                        Debug.LogError($"Can't find required component in ability owner (ComponentType={nameof(Size)})");
                        return;
                    }

                    var startPosition = position.Value;
                    startPosition.y += size.Value.y * 0.5f;

                    for (var i = 0; i < spawnAbility.ProjectilesAmount; i++)
                    {
                        var projectile = world.Create();
                        commandBuffer.Add(projectile, projectileData);
                        commandBuffer.Add(projectile, locomotionData);
                        
                        commandBuffer.Add(projectile, new Position { Value = position.Value });
                        commandBuffer.Add(projectile, new Rotation { Value = Quaternion.identity });
                        commandBuffer.Add(projectile, new ProjectileState());
                        commandBuffer.Add(projectile, new IgnoreRotationTag());
                        commandBuffer.Add(projectile, new LocomotionState
                        {
                            Speed = locomotionData.MaxSpeed,
                            Direction = GetDirection(rotation.Value, spawnAbility.SpawnDirection),
                            LastPosition = position.Value
                        });

                        entity.CopyComponentToEntityIfExists<Damage>(projectile);
                        entity.CopyComponentToEntityIfExists<PrefabId>(projectile);
                        ownerEntity.CopyComponentToEntityIfExists<Fraction>(projectile);
                    }

                    ability.LastActivateTime = Context.Time;
                    commandBuffer.Remove<AbilityReadyTag>(entity);
                });
        }

        private static Vector3 GetDirection(Quaternion rotation, SpawnDirectionType spawnDirectionType)
        {
            if (spawnDirectionType == SpawnDirectionType.Forward)
            {
                return rotation * Vector3.forward; 
            }
            
            var angle = Random.value * Mathf.PI * 2;
            return new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
        }
    }
}