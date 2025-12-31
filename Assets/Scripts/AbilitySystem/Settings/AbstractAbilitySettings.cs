using UnityEngine;

namespace Game.AbilitySystem.Settings
{
    public abstract class AbstractAbilitySettings : ScriptableObject
    {
        public float ActivateTimeout;

        public abstract IAbility Instantiate();
    }
}