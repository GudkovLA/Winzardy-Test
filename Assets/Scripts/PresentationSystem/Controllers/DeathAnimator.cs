#nullable enable

using UnityEngine;
using UnityEngine.UIElements.Experimental;

namespace Game.PresentationSystem.Controllers
{
    public class DeathAnimator : MonoBehaviour
    {
        [SerializeField]
        private Transform _body = null!;
        
        [SerializeField]
        private float _duration;

        private bool _isDead;
        private float _deathTime;
        private float _progress;

        public float Duration => _duration;
        
        public void SetDeathTime(float value)
        {
            if (_isDead)
            {
                return;
            }
            
            _isDead = true;
            _deathTime = value;
        }

        private void OnEnable()
        {
            _isDead = false;
            _deathTime = 0;
            _body.localScale = Vector3.one;
        }

        private void Update()
        {
            if (!_isDead)
            {
                return;
            }

            _progress = _duration > 0 
                ? Mathf.Clamp01((Time.realtimeSinceStartup - _deathTime) / _duration) 
                : 1;

            _body.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, Easing.InCubic(_progress));
        }
    }
}