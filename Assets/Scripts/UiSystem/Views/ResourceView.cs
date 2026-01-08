#nullable enable

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UiSystem.Views
{
    public class ResourceView : MonoBehaviour
    {
        [SerializeField]
        private Image _icon = null!;

        [SerializeField]
        private TextMeshProUGUI _amountLabel = null!;

        [SerializeField] 
        private float _duration; 

        [SerializeField] 
        private float _maxScale; 

        private int _currentAmount; 
        private float _increaseTime; 

        public void SetIcon(Sprite? sprite)
        {
            if (_icon.sprite != sprite)
            {
                _icon.sprite = sprite;
            }
        }

        public void SetAmount(int value)
        {
            if (value > _currentAmount)
            {
                _amountLabel.transform.localScale = Vector3.one * _maxScale;
                _increaseTime = Time.realtimeSinceStartup;
            }
            
            _amountLabel.text = value.ToString();
            _currentAmount = value;
        }

        private void Awake()
        {
            _currentAmount = 0;
            _amountLabel.text = _currentAmount.ToString();
        }

        private void Update()
        {
            var timePassed = Time.realtimeSinceStartup - _increaseTime;
            var progress = Mathf.Clamp01(timePassed / _duration);
            _amountLabel.transform.localScale = Vector3.one * Mathf.Lerp(1f, _maxScale, 1 - progress);
        }
    }
}
