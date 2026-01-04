#nullable enable

using System;
using Game.SpawnSystem;
using UnityEngine;

namespace Game.Settings
{
    [CreateAssetMenu(fileName = nameof(GameSettings), menuName = "Assets/Game Settings")]
    [Serializable]
    public class GameSettings : ScriptableObject, IDisposable
    {
        public CameraSettingsData CameraSettings = new();
        public SpawnSettingsData SpawnSettings = new();
        public GameObject? HealthViewPrefab;
        public int HealthViewPoolSize;
        
        public void Dispose()
        {
        }

        [Serializable]
        public class CameraSettingsData
        {
            public Vector3 Angle;
            public Vector3 Offset;
        }
 
        [Serializable]
        public class SpawnSettingsData
        {
            public float EnemySpawnTimeout;
            public float CameraAreaScale;
            public float AreaMaxDepth;
            public float SpawnRetryCountLimit;
            public float SpawnCollisionSafeRadius;

            public bool DebugEnable;

            public SpawnAreaWeight[] SpawnAreas;
        }

        [Serializable]
        public class SpawnAreaWeight
        {
            public SpawnAreaId AreaId;
            public float Weight;
        }
    }
}