#nullable enable

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UiSystem
{
    public class HealthController : MonoBehaviour
    {
        [SerializeField]
        public TextMeshProUGUI? _label;
        
        [SerializeField]
        public Slider? _slider;

        public void SetHealth(float value)
        {
            if (_label != null)
            {
                _label.text = Mathf.CeilToInt(value).ToString();
            }

            if (_slider != null)
            {
                _slider.value = value;
            }
        }

        public void SetMaxHealth(float value)
        {
            if (_slider != null)
            {
                _slider.maxValue = value;
            }
        }

        private void Awake()
        {
            if (_label != null)
            {
                _label.text = "";
            }

            if (_slider != null)
            {
                _slider.value = 0;
                _slider.maxValue = 0;
            }
        }
    }
}