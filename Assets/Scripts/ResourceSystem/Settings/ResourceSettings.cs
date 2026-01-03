#nullable enable

using System;
using UnityEngine;

namespace Game.ResourceSystem.Settings
{
    [CreateAssetMenu(fileName = nameof(ResourceSettings), menuName = "Assets/Resource Settings")]
    [Serializable]
    public class ResourceSettings : ScriptableObject, IDisposable
    {
        public GameObject? Prefab;
        public Sprite? Icon;

        public void Dispose()
        {
        }
    }
}