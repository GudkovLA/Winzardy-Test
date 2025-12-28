using TMPro;
using UnityEngine;

namespace Game.Ui
{
    public class ResourceController : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _coinsAmountLabel;

        [SerializeField] 
        private float _duration; 

        [SerializeField] 
        private float _maxScale; 

        private int _currentAmount; 
        private float _increaseTime; 

        public void SetAmount(int value)
        {
            if (value > _currentAmount)
            {
                _coinsAmountLabel.transform.localScale = Vector3.one * _maxScale;
                _increaseTime = Time.realtimeSinceStartup;
            }
            
            _coinsAmountLabel.text = value.ToString();
            _currentAmount = value;
        }

        private void Awake()
        {
            _currentAmount = 0;
            _coinsAmountLabel.text = _currentAmount.ToString();
        }

        private void Update()
        {
            var timePassed = Time.realtimeSinceStartup - _increaseTime;
            var progress = Mathf.Clamp01(timePassed / _duration);
            _coinsAmountLabel.transform.localScale = Vector3.one * Mathf.Lerp(1f, _maxScale, 1 - progress);
        }
    }
}
