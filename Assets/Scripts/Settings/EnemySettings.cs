#nullable enable

using System;
using UnityEngine;

namespace Game.Settings
{
    [CreateAssetMenu(fileName = nameof(EnemySettings), menuName = "Assets/Enemy Settings")]
    [Serializable]
    public class EnemySettings : ScriptableObject, IDisposable
    {
        public GameObject Prefab;
        public float Speed;
        public Vector3 Size;
        public float MaxHealth;
        public float HitTimeout;

        // TODO: Possible to make several coins
        public CoinSettingsData CoinSettings;
        
        public DamageSettingsData DamageSettings;
        
        public void Dispose()
        {
        }
        
        [Serializable]
        public class CoinSettingsData
        {
            public GameObject Prefab;
            
            [Tooltip("Chance of coin will be dropped, where 1 means 100% of chance")]
            [Range(0, 1)]
            public float DropChance;
        }
        
        [Serializable]
        public class DamageSettingsData
        {
            public float Amount;
            public float HitDistance;
        }
    }
}