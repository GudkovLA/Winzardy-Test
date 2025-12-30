#nullable enable

using System;
using Arch.Core;
using Arch.Core.Extensions;
using Game.Common.Systems;
using Game.Common.Systems.Attributes;
using Game.Components;
using Game.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class EnemyMovementSystem : AbstractSystem
    {
        private const float kAvoidanceDirectionChangeTimeout = 1f;
        private const float kRotationSpeed = 1f;

        private static readonly float _radiusFactor = Mathf.Sqrt(2) * 0.5f;
        private static readonly int[] _randomDirectionFactor = { -1, 1 };

        private static readonly QueryDescription _enemyInitQuery = new QueryDescription()
            .WithAll<Position, Rotation, Size, Enemy>()
            .WithNone<ObstacleAvoidance, Destroy>();

        private static readonly QueryDescription _enemyQuery = new QueryDescription()
            .WithAll<Position, Rotation, Size, Enemy, ObstacleAvoidance>()
            .WithNone<Destroy>();
        
        private readonly Collider[] _colliders = new Collider[4];  

        protected override void OnUpdate()
        {
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
                    ref Rotation rotation, 
                    ref Size size, 
                    ref Enemy enemy, 
                    ref ObstacleAvoidance obstacleAvoidance) =>
                {
                    var delta = playerPosition.Value - position.Value;
                    delta.y = 0;

                    var newPosition = position.Value;
                    var radius = _radiusFactor * size.Value.x;
                    
                    // Calculate the direction of movement in accordance with obstacles on the way to the goal.
                    // Simple algorithm for bypassing an obstacle in the selected direction (left handed or right handed). 
                    var direction = ResolveMovementDirection(position.Value, 
                        size.Value, 
                        delta.normalized,
                        radius,
                        obstacleAvoidance.DirectionFactor);

                    newPosition += direction.normalized * (enemy.Speed * Context.DeltaTime);
                    
                    // Detects collisions with obstacles to avoid crossing their boundaries.
                    // If the enemy comes into contact with an obstacle, it will be pushed beyond its boundaries. 
                    var collisionCorrection = ResolveColliderOverlap(newPosition, radius, _colliders);
                    newPosition += collisionCorrection + collisionCorrection * Context.DeltaTime;

                    // A small workaround to prevent enemies from getting stuck inside the corners.
                    // This can happen when an enemy tries to move toward an obstacle (while avoiding another obstacle),
                    // but is pushed back by collision correction.
                    // When the enemy's movement speed becomes too slow, it changes the direction in which it avoids the obstacle.
                    var movementThreshold = enemy.Speed * Context.DeltaTime * 0.5f;
                    if (Vector3.Distance(newPosition, position.Value) < movementThreshold)
                    {
                        if (obstacleAvoidance.ChangeTime + kAvoidanceDirectionChangeTimeout < Context.Time)
                        {
                            obstacleAvoidance.DirectionFactor *= -1;
                            obstacleAvoidance.ChangeTime = Context.Time;
                        }
                    }

                    position.Value = newPosition;
                    rotation.Value = Quaternion.RotateTowards(
                        rotation.Value,
                        Quaternion.LookRotation(direction, Vector3.up),
                        360f * Context.DeltaTime * kRotationSpeed
                    );
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
                || !Physics.SphereCast(castFrom, radius, targetDirection, out var hitPoint, maxDistance))
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

        private static Vector3 ResolveColliderOverlap(Vector3 position, float radius, Collider[] collidersCache)
        {
            var collisionCorrection  = Vector3.zero;
            var count = Physics.OverlapSphereNonAlloc(position, radius, collidersCache);
            for (var i = 0; i < count; i++)
            {
                var closest = collidersCache[i].ClosestPoint(position);
                var delta = position - closest;
                delta.y = 0;

                collisionCorrection += delta.normalized * Math.Abs(radius - delta.magnitude);
            }
            
            return collisionCorrection;
        }

        private struct ObstacleAvoidance
        {
            public int DirectionFactor;
            public float ChangeTime;
        }
    }
}