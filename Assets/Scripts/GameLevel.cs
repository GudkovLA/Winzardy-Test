using System;
using UnityEngine;

namespace Game
{
    public class GameLevel : IDisposable
    {
        private readonly Plane _groundPlane = new Plane(Vector3.up, Vector3.zero); 
            
        public Transform Root { private set; get; }
        public Vector3 StartPosition { private set; get; }
        public Quaternion StartRotation { private set; get; }

        public Plane GroundPlane => _groundPlane;

        public GameLevel(Transform root, Vector3 startPosition, Quaternion startRotation)
        {
            Root = root;
            StartPosition = startPosition;
            StartRotation = startRotation;
        }

        public void Dispose()
        {
            Root =  null;
        }
    }
}