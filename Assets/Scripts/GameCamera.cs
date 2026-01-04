#nullable enable

using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Game
{
    public class CameraBoundaries
    {
        public readonly Vector3[] Corners = new Vector3[4];
            
        public Vector3 NearLeft => Corners[0];
        public Vector3 FarLeft => Corners[1];
        public Vector3 FarRight => Corners[2];
        public Vector3 NearRight => Corners[3];
    }
    
        
    public class GameCamera : IDisposable
    {
        public Camera Camera { private set; get; }

        private readonly Vector3[] _nearCorners = new Vector3[4];
        private readonly Vector3[] _farCorners  = new Vector3[4];

        public GameCamera(Camera camera)
        {
            Camera = camera;
        }

        public void SetTransform(Vector3 position, Quaternion rotation)
        {
            var transform = Camera.transform;
            transform.position = position;
            transform.rotation = rotation;
        }

        public void UpdateCameraBoundaries(Plane ground, CameraBoundaries cameraBoundaries)
        {
            GeometryUtility.CalculateFrustumPlanes(Camera);

            Camera.CalculateFrustumCorners(
                new Rect(0, 0, 1, 1),
                Camera.nearClipPlane,
                Camera.MonoOrStereoscopicEye.Mono,
                _nearCorners
            );

            Camera.CalculateFrustumCorners(
                new Rect(0, 0, 1, 1),
                Camera.farClipPlane,
                Camera.MonoOrStereoscopicEye.Mono,
                _farCorners
            );
            
            for (var i = 0; i < 4; i++)
            {
                _nearCorners[i] = Camera.transform.TransformPoint(_nearCorners[i]);
                _farCorners[i]  = Camera.transform.TransformPoint(_farCorners[i]);
            }

            cameraBoundaries.Corners[0] = Intersect(ground, _nearCorners[0], _farCorners[0]); // Near Left
            cameraBoundaries.Corners[1] = Intersect(ground, _nearCorners[1], _farCorners[1]); // Far Left
            cameraBoundaries.Corners[2] = Intersect(ground, _nearCorners[2], _farCorners[2]); // Far Right
            cameraBoundaries.Corners[3] = Intersect(ground, _nearCorners[3], _farCorners[3]); // Near Right
        }

        public void Dispose()
        {
            Camera = null!;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Vector3 Intersect(Plane plane, Vector3 a, Vector3 b)
        {
            var r = new Ray(a, b - a);
            plane.Raycast(r, out var distance);
            return r.GetPoint(distance);
        }            
    }
}