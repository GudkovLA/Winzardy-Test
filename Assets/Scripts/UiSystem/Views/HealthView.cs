#nullable enable

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UiSystem.Views
{
    public class HealthView : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI? _label;
        
        [SerializeField]
        private Slider? _slider;

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