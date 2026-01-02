#nullable enable

using System;
using Arch.Core;
using Game.Common.Components;
using Game.Common.Systems;
using Game.Common.Systems.Attributes;
using Game.LocomotionSystem.Components;
using UnityEngine;

namespace Game.LocomotionSystem.Systems
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public class LocomotionUpdateSystem : AbstractSystem
    {
        private const float kRotationSpeed = 1f;
        private static readonly float _radiusFactor = Mathf.Sqrt(2) * 0.5f;

        private static readonly QueryDescription _locomotionQuery = new QueryDescription()
            .WithAll<Position, LocomotionState>()
            .WithNone<Destroy>();
        
        private static readonly QueryDescription _collisionQuery = new QueryDescription()
            .WithAll<Position, Size>()
            .WithNone<Destroy, IgnoreObstaclesTag>();

        private static readonly QueryDescription _rotationQuery = new QueryDescription()
            .WithAll<Position, Rotation, LocomotionState>()
            .WithNone<Destroy, IgnoreRotationTag>();
        
        private readonly Collider[] _colliders = new Collider[4];  

        protected override void OnUpdate()
        {
            // Move Entity according its movement direction and sped
            World.Query(_locomotionQuery,
                (ref Position position, ref LocomotionState locomotionState) =>
                {
                    var velocity = locomotionState.Direction * (locomotionState.Speed * Context.DeltaTime);
                    locomotionState.LastPosition = position.Value;
                    locomotionState.LastVelocity = velocity;
                    position.Value += velocity;
                });

            // Detects collisions with obstacles to avoid crossing their boundaries.
            // If the Entity comes into contact with an obstacle, it will be pushed beyond its boundaries. 
            World.Query(_collisionQuery,
                (ref Position position, ref Size size) =>
                {
                    var radius = _radiusFactor * size.Value.x;
                    position.Value += ResolveColliderOverlap(position.Value, radius, _colliders);
                });
            
            // Update rotation by moving direction
            var rotationSpeed = 360f * Context.DeltaTime * kRotationSpeed;
            World.Query(_rotationQuery,
                (ref Position position, ref Rotation rotation, ref LocomotionState locomotionState) =>
                {
                    var direction = (position.Value - locomotionState.LastPosition).normalized;
                    var lookDirection = direction != Vector3.zero
                        ? Quaternion.LookRotation(locomotionState.Direction, Vector3.up)
                        : Quaternion.identity;

                    rotation.Value = Quaternion.RotateTowards(rotation.Value, lookDirection, rotationSpeed);
                });
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
    }
}