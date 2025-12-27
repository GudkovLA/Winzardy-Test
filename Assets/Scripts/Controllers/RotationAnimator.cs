using UnityEngine;

namespace Game.Controllers
{
    public class RotationAnimator : MonoBehaviour
    {
        [SerializeField]
        private Vector3 _axisSpeed;
        
        private void Update()
        {
            var transformCache = transform;
            var rotation = _axisSpeed * Time.deltaTime;
            transformCache.rotation = Quaternion.Euler(rotation) * transformCache.rotation;
        }
    }
}
