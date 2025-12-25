using System;
using UnityEngine;

namespace Game
{
    public class GameCamera : IDisposable
    {
        public Camera Camera { private set; get; }

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

        public void Dispose()
        {
            Camera = null;
        }
    }
}