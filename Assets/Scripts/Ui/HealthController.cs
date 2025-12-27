using TMPro;
using UnityEngine;

namespace Game.Ui
{
    public class HealthController : MonoBehaviour
    {
        [SerializeField]
        public TextMeshProUGUI _label;
        
        public void SetHealth(float health)
        {
            _label.text = Mathf.CeilToInt(health).ToString();
        }
    }
}