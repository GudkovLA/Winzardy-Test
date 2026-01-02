#nullable enable

using System;
using Game.AbilitySystem.Settings;
using Game.CharacterSystem.Components;
using UnityEngine;

namespace Game.CharacterSystem.Settings
{
    [CreateAssetMenu(fileName = nameof(PlayerSettings), menuName = "Assets/Player Settings")]
    [Serializable]
    public class PlayerSettings : ScriptableObject, IDisposable
    {
        public CharacterSettings Character;
        public int PoolSize;

        public FractionMask Fraction; 
        public FractionMask Enemies; 

        public float CoinsCollectRadius;

        public AbstractAbilitySettings[] Abilities;

        public void Dispose()
        {
        }
    }
}