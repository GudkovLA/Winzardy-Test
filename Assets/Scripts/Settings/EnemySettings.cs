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
        public Vector3 Size;
        public float MaxHealth;
        
        public void Dispose()
        {
        }
    }
}