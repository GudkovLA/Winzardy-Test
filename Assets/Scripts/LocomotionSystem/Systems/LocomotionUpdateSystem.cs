#nullable enable

using Arch.Core;
using Game.Common;
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
        private readonly QueryDescription _locomotionQuery = new QueryDescription()
            .WithAll<Position, LocomotionState>()
            .WithNone<Destroy>();
        
        private readonly QueryDescription _collisionQuery = new QueryDescription()
            .WithAll<Position, Size>()
            .WithNone<Destroy, IgnoreObstaclesTag>();

        private readonly QueryDescription _rotationQuery = new QueryDescription()
            .WithAll<Position, Rotation, LocomotionState>()
            .WithNone<Destroy, IgnoreRotationTag>();
        
        private readonly Collider[] _colliders = new Collider[4];  
        private readonly float _radiusFactor = Mathf.Sqrt(2) * 0.5f;

        private float _turnSpeed;

        protected override void OnCreate()
        {
            base.OnCreate();

            _turnSpeed = ServiceLocator.TryGet<GameSettings>(out var gameSettings)
                ? gameSettings.LocomotionTurnSpeed
                : 1f;
        }

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
            var rotationSpeed = 360f * Context.DeltaTime * _turnSpeed;
            World.Query(_rotationQuery,
                (ref Position position, ref Rotation rotation, ref LocomotionState locomotionState) =>
                {
                    var direction = (position.Value - locomotionState.LastPosition).normalized;
                    if (direction == Vector3.zero)
                    {
                        direction = locomotionState.Direction;
                    }
                    
                    var lookDirection = direction != Vector3.zero
                        ? Quaternion.LookRotation(direction, Vector3.up)
                        : Quaternion.identity;

                    rotation.Value = Quaternion.RotateTowards(rotation.Value, lookDirection, rotationSpeed);
                });
        }

        private static Vector3 ResolveColliderOverlap(Vector3 position, float radius, Collider[] collidersCache)
        {
            var collisionCorrection  = Vector3.zero;
            var count = Physics.OverlapSphereNonAlloc(position, radius, collidersCache, (int) PhysicsLayer.Obstacle);
            for (var i = 0; i < count; i++)
            {
                var closest = collidersCache[i].ClosestPoint(position);
                var delta = position - closest;
                delta.y = 0;

                collisionCorrection += delta.normalized * Mathf.Abs(radius - delta.magnitude);
            }
            
            return collisionCorrection;
        }
    }
}