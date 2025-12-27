using UnityEngine;

namespace Game.Controllers
{
    public class HitAnimator : MonoBehaviour
    {
        [SerializeField]
        private float _duration;

        [SerializeField]
        private Renderer _renderer;
        
        private Color _initialColor;
        private float _lastHitTime;
        
        public void UpdateLastHit(float lastHitTime)
        {
            _lastHitTime = lastHitTime;
        }

        private void OnEnable()
        {
            if (_renderer != null)
            {
                _initialColor = _renderer.material.color;
            }
        }

        private void OnDisable()
        {
            if (_renderer != null)
            {
                _renderer.material.color = _initialColor;
            }
        }

        private void Update()
        {
            if (_renderer == null)
            {
                return;
            }

            var timePassed = Time.realtimeSinceStartup - _lastHitTime;
            var progress = Mathf.Clamp01(timePassed / _duration);
            _renderer.material.color = Color.Lerp(_initialColor, Color.white, 1f - progress);
        }
    }
}