#nullable enable

using Arch.Core;
using Arch.Core.Extensions;
using Game.CharacterSystem.Components;
using Game.Common;
using Game.Common.Components;
using Game.Common.Systems;
using Game.Common.Systems.Attributes;
using Game.DamageSystem.Components;
using Game.LocomotionSystem.Components;
using Game.Utils;
using UnityEngine;

namespace Game.CharacterSystem.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class EnemyControlSystem : AbstractSystem
    {
        private readonly QueryDescription _enemiesQuery = new QueryDescription()
            .WithAll<Position, Size, LocomotionData, LocomotionState, EnemyControlState>()
            .WithNone<Destroy, IgnoreObstaclesTag, DeathState>();

        private readonly QueryDescription _deadEnemyQuery = new QueryDescription()
            .WithAll<LocomotionState, EnemyControlState, DeathState>()
            .WithNone<Destroy>();

        private float _avoidanceDirectionChangeTimeout;

        protected override void OnCreate()
        {
            base.OnCreate();

            _avoidanceDirectionChangeTimeout = ServiceLocator.TryGet<GameSettings>(out var gameSettings)
                ? gameSettings.AvoidanceDirectionChangeTimeout
                : 1f;
        }

        protected override void OnUpdate()
        {
            var playerEntity = World.GetPlayerSingleton();
            if (playerEntity == Entity.Null
                || !playerEntity.TryGet<Position>(out var playerPosition))
            {
                return;
            }

            var time = Context.Time;
            World.Query(_enemiesQuery,
                (ref Position position, 
                    ref Size size, 
                    ref LocomotionData locomotionData,
                    ref LocomotionState locomotionState,
                    ref EnemyControlState controlState) =>
                {
                    // A small workaround to prevent enemies from getting stuck inside the corners.
                    // This can happen when an enemy tries to move toward an obstacle (while avoiding another obstacle),
                    // but is pushed back by collision correction.
                    // When the enemy's movement speed becomes too slow, it changes the direction in which it avoids the obstacle.
                    var movementThreshold = locomotionState.LastVelocity.magnitude * 0.5f;
                    if (Vector3.Distance(locomotionState.LastPosition, position.Value) < movementThreshold)
                    {
                        if (controlState.DirectionChangeLastTime + _avoidanceDirectionChangeTimeout < time)
                        {
                            controlState.ObstacleAvoidanceDirection *= -1;
                            controlState.DirectionChangeLastTime = time;
                        }
                    }
                    
                    var delta = playerPosition.Value - position.Value;
                    delta.y = 0;

                    // Don't move when close enough to player
                    locomotionState.Speed = delta.magnitude > controlState.MinDistanceToPlayer
                        ? locomotionData.MaxSpeed
                        : 0f;
                    
                    // Calculate the direction of movement in accordance with obstacles on the way to the goal.
                    // Simple algorithm for bypassing an obstacle in the selected direction (left handed or right handed). 
                    locomotionState.Direction = ResolveMovementDirection(position.Value, 
                        delta.normalized,
                        size.Radius,
                        size.Height,
                        controlState.ObstacleAvoidanceDistance, 
                        controlState.ObstacleAvoidanceDirection).normalized;
                });
            
            World.Query(_deadEnemyQuery,
                (ref LocomotionState locomotionState) =>
                {
                    locomotionState.Speed = 0f;
                    locomotionState.Direction = Vector3.zero;
                });
        }

        private static Vector3 ResolveMovementDirection(
            Vector3 position, 
            Vector3 targetDirection,
            float radius,
            float height,
            float avoidanceDistance,
            int avoidanceFactor)
        {
            var castFrom = new Vector3(position.x, height * 0.5f, position.z);
            
            if (avoidanceDistance <= 0
                || !Physics.SphereCast(castFrom, 
                    radius, 
                    targetDirection, 
                    out var hitPoint, 
                    avoidanceDistance, 
                    (int) PhysicsLayer.Obstacle))
            {
                return targetDirection;
            }

            var avoidanceDirection = Vector3.Cross(hitPoint.normal,
                avoidanceFactor == 1
                    ? Vector3.up
                    : Vector3.down).normalized;

            return targetDirection * (hitPoint.distance / avoidanceDistance)
                        + avoidanceDirection * ((avoidanceDistance - hitPoint.distance) / avoidanceDistance);
        }
    }
}