using UnityEngine;

namespace Game.Ui
{
    public class HudController : MonoBehaviour
    {
        [SerializeField]
        private ResourceController _coinsAmount;

        public void SetCoinsAmount(int value)
        {
            _coinsAmount.SetAmount(value);
        }
    }
}