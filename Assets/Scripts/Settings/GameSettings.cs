#nullable enable

using System;
using UnityEngine;

namespace Game.Settings
{
    [CreateAssetMenu(fileName = nameof(GameSettings), menuName = "Assets/Game Settings")]
    [Serializable]
    public class GameSettings : ScriptableObject, IDisposable
    {
        public CameraSettingsData CameraSettings = new();
        
        public void Dispose()
        {
        }

        [Serializable]
        public class CameraSettingsData
        {
            public Vector3 Angle;
            public Vector3 Offset;
        }
    }
}