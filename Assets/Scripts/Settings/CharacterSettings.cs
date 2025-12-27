#nullable enable

using System;
using UnityEngine;

namespace Game.Settings
{
    [CreateAssetMenu(fileName = nameof(CharacterSettings), menuName = "Assets/Character Settings")]
    [Serializable]
    public class CharacterSettings : ScriptableObject, IDisposable
    {
        public GameObject Prefab;
        public ProjectileSettings Projectile;
        public float MaxHealth;
        public float Speed;
        public float CoinsCollectRadius;

        public void Dispose()
        {
        }
    }
}