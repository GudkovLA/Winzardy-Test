#nullable enable

using System;
using UnityEngine;

namespace Game.CharacterSystem.Settings
{
    [CreateAssetMenu(fileName = nameof(CharacterSettings), menuName = "Assets/Character Settings")]
    [Serializable]
    public class CharacterSettings : ScriptableObject, IDisposable
    {
        public GameObject? Prefab;
        
        public Vector3 Size;
        public float ColliderRadius; 
        
        public float MaxHealth;
        public float Speed;

        public void Dispose()
        {
        }
    }
}