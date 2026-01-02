#nullable enable

using System;
using Game.AbilitySystem.Settings;
using Game.Components;
using UnityEngine;

namespace Game.Settings
{
    [CreateAssetMenu(fileName = nameof(CharacterSettings), menuName = "Assets/Character Settings")]
    [Serializable]
    public class CharacterSettings : ScriptableObject, IDisposable
    {
        public GameObject? Prefab;
        public int PoolSize;
        
        public FractionMask Fraction; 
        public FractionMask Enemies; 
        
        public Vector3 Size;
        public float MaxHealth;
        public float Speed;
        public float CoinsCollectRadius;
        public float ColliderRadius; 

        public AbstractAbilitySettings[] Abilities;

        public void Dispose()
        {
        }
    }
}