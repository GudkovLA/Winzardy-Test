#nullable enable

using System;
using Game.CharacterSystem.Settings;
using Game.SpawnSystem;
using Game.Utils;
using UnityEngine;

namespace Game.Settings
{
    [CreateAssetMenu(fileName = nameof(GameSettings), menuName = "Assets/Game Settings")]
    [Serializable]
    public class GameSettings : ScriptableObject, IDisposable, IPoolable
    {
        public CameraSettingsData Camera = null!;
        public SpawnSettingsData Spawn = null!;
        public EnemySettingsData[] Enemies = null!;
        public PlayerSettings Player = null!;

        public GameObject? HealthViewPrefab;
        public int HealthViewPoolSize;
        
        public void Dispose()
        {
        }

        public void Prepare(InstancePool instancePool)
        {
            if (HealthViewPrefab != null)
            {
                instancePool.Register(HealthViewPrefab, HealthViewPoolSize);
            }
            
            Player.Prepare(instancePool);
            foreach (var enemySettingsData in Enemies)
            {
                enemySettingsData.EnemySettings.Prepare(instancePool);
            }
        }

        [Serializable]
        public class CameraSettingsData
        {
            public Vector3 Angle;
            public Vector3 Offset;
        }

        [Serializable]
        public class EnemySettingsData
        {
            public EnemySettings EnemySettings = null!;
            public float SpawnTimeout;
            public float FirstSpawnTimeout;
        }
 
        [Serializable]
        public class SpawnSettingsData
        {
            public float CameraAreaScale;
            public float AreaMaxDepth;
            public float SpawnRetryCountLimit;
            public float SpawnCollisionSafeRadius;

            public bool DebugEnable;

            public SpawnAreaWeight[] SpawnAreas = null!;
        }

        [Serializable]
        public class SpawnAreaWeight
        {
            public SpawnAreaId AreaId;
            public float Weight;
        }
    }
}