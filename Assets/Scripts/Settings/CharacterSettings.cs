#nullable enable

using System;
using Game.AbilitySystem.Settings;
using UnityEngine;

namespace Game.Settings
{
    [CreateAssetMenu(fileName = nameof(CharacterSettings), menuName = "Assets/Character Settings")]
    [Serializable]
    public class CharacterSettings : ScriptableObject, IDisposable
    {
        public GameObject Prefab;
        public int PoolSize;
        
        public AbstractAbilitySettings[] Abilities;
        
        public float MaxHealth;
        public float Speed;
        public float CoinsCollectRadius;

        public void Dispose()
        {
        }
    }
}