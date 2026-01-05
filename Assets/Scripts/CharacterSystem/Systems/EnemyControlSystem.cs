#nullable enable

using Arch.Core;
using Arch.Core.Extensions;
using Game.CharacterSystem.Components;
using Game.Common;
using Game.Common.Components;
using Game.Common.Systems;
using Game.Common.Systems.Attributes;
using Game.LocomotionSystem.Components;
using Game.Settings;
using Game.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.CharacterSystem.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class EnemyControlSystem : AbstractSystem
    {
        private const float kAvoidanceDirectionChangeTimeout = 1f;

        private static readonly float _radiusFactor = Mathf.Sqrt(2) * 0.5f;
        private static readonly int[] _randomDirectionFactor = { -1, 1 };

        private static readonly QueryDescription _enemyInitQuery = new QueryDescription()
            .WithAll<Position, LocomotionState, EnemyControlState>()
            .WithNone<Destroy, ObstacleAvoidance>();

        private static readonly QueryDescription _enemyQuery = new QueryDescription()
            .WithAll<Position, Size, LocomotionData, LocomotionState, EnemyControlState, ObstacleAvoidance>()
            .WithNone<Destroy>();

        private GameSettings _gameSettings = null!;
        private bool _initialized;
        
        protected override void OnCreate()
        {
            base.OnCreate();
            
            if (!ServiceLocator.TryGet(out _gameSettings))
            {
                return;
            }

            _initialized = true;
        }
        
        protected override void OnUpdate()
        {
            if (!_initialized)
            {
                return;
            }
            
            var playerEntity = World.GetPlayerSingleton();
            if (playerEntity == Entity.Null
                || !playerEntity.TryGet<Position>(out var playerPosition))
            {
                return;
            }

            var commandBuffer = Context.GetOrCreateCommandBuffer(this);
            World.Query(_enemyInitQuery,
                entity =>
                {
                    var directionIndex = Random.Range(0, _randomDirectionFactor.Length);
                    commandBuffer.Add(entity, new ObstacleAvoidance
                    {
                        DirectionFactor = _randomDirectionFactor[directionIndex]
                    });
                });

            World.Query(_enemyQuery,
                (ref Position position, 
                    ref Size size, 
                    ref LocomotionData locomotionData,
                    ref LocomotionState locomotionState,
                    ref EnemyControlState controlState,
                    ref ObstacleAvoidance obstacleAvoidance) =>
                {
                    // A small workaround to prevent enemies from getting stuck inside the corners.
                    // This can happen when an enemy tries to move toward an obstacle (while avoiding another obstacle),
                    // but is pushed back by collision correction.
                    // When the enemy's movement speed becomes too slow, it changes the direction in which it avoids the obstacle.
                    var movementThreshold = locomotionState.LastVelocity.magnitude * 0.5f;
                    if (Vector3.Distance(locomotionState.LastPosition, position.Value) < movementThreshold)
                    {
                        if (obstacleAvoidance.LastChangeTime + kAvoidanceDirectionChangeTimeout < Context.Time)
                        {
                            obstacleAvoidance.DirectionFactor *= -1;
                            obstacleAvoidance.LastChangeTime = Context.Time;
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
                        size.Value, 
                        delta.normalized,
                        _radiusFactor * size.Value.x,
                        obstacleAvoidance.DirectionFactor);
                });
        }

        private static Vector3 ResolveMovementDirection(
            Vector3 position, 
            Vector3 size, 
            Vector3 targetDirection,
            float radius,
            int avoidanceFactor)
        {
            var castFrom = new Vector3(position.x, size.y * 0.5f, position.z);
            var maxDistance = size.x;
            
            if (maxDistance <= 0
                || !Physics.SphereCast(castFrom, 
                    radius, 
                    targetDirection, 
                    out var hitPoint, 
                    maxDistance, 
                    (int) PhysicsLayer.Obstacle))
            {
                return targetDirection;
            }

            var avoidanceDirection = Vector3.Cross(hitPoint.normal,
                avoidanceFactor == 1
                    ? Vector3.up
                    : Vector3.down).normalized;

            return targetDirection * (hitPoint.distance / maxDistance)
                        + avoidanceDirection * ((maxDistance - hitPoint.distance) / maxDistance);
        }

        private struct ObstacleAvoidance
        {
            public int DirectionFactor;
            public float LastChangeTime;
        }
    }
}