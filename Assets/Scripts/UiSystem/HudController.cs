using UnityEngine;

namespace Game.Ui
{
    public class HudController : MonoBehaviour
    {
        [SerializeField]
        private ResourceController _coinsAmount;

        [SerializeField]
        private HealthController _healthAmount;

        public void SetCoinsAmount(int value)
        {
            _coinsAmount.SetAmount(value);
        }

        public void SetHealthAmount(float value, float maxValue)
        {
            _healthAmount.SetMaxHealth(maxValue);
            _healthAmount.SetHealth(value);
        }
    }
}