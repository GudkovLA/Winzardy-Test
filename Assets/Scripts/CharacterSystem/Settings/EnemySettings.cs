#nullable enable

using System;
using Game.AbilitySystem.Settings;
using Game.CharacterSystem.Components;
using UnityEngine;

namespace Game.CharacterSystem.Settings
{
    [CreateAssetMenu(fileName = nameof(EnemySettings), menuName = "Assets/Enemy Settings")]
    [Serializable]
    public class EnemySettings : ScriptableObject, IDisposable
    {
        public CharacterSettings Character;
        public int PoolSize;
        
        public FractionMask Fraction; 
        public FractionMask Enemies; 

        // TODO: Possible to make several coins
        public CoinSettingsData CoinSettings;
        
        public AbstractAbilitySettings[] Abilities;
        
        public void Dispose()
        {
        }
        
        [Serializable]
        public class CoinSettingsData
        {
            public GameObject Prefab;
            public int PoolSize;
            
            [Tooltip("Chance of coin will be dropped, where 1 means 100% of chance")]
            [Range(0, 1)]
            public float DropChance;
        }
    }
}