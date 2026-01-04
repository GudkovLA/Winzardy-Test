using UnityEngine;

namespace Game.LocomotionSystem.Components
{
    public struct LocomotionState
    {
        public Vector3 Direction;
        public float MaxSpeed;
        public float Speed;

        public Vector3 LastPosition;
        public Vector3 LastVelocity;
    }
}