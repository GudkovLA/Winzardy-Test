#nullable enable

using System.Collections.Generic;
using Game.Common;
using Game.Utils;
using UnityEngine;

namespace Game.UiSystem.Controllers
{
    public class HudController : MonoBehaviour
    {
        private readonly List<ResourceController> _resourceViews = new() ;

        [SerializeField]
        private GameObject _resourceViewPrefab = null!;

        [SerializeField]
        private Transform _resourceViewRoot = null!;

        [SerializeField]
        private HealthController _healthAmount = null!;

        private InstancePool _instancePool = null!;
        
        public void InitializeFrom(ServiceLocator serviceLocator)
        {
            _instancePool = serviceLocator.GetRequired<InstancePool>();
            _instancePool.Register(_resourceViewPrefab, 2);
        }

        public void StartGame()
        {
        }
        
        public ResourceController GetResourceView(int index)
        {
            if (index < _resourceViews.Count)
            {
                return _resourceViews[index];
            }

            while (index >= _resourceViews.Count)
            {
                var instance = _instancePool.Get(_resourceViewPrefab.GetInstanceID());
                if (instance != null)
                {
                    instance.transform.SetParent(_resourceViewRoot);
                    instance.SetActive(true);
                
                    var resourceView = instance.GetComponent<ResourceController>(); 
                    _resourceViews.Add(resourceView);
                }
            }

            return _resourceViews[index];
        }

        public void SetHealthAmount(float value, float maxValue)
        {
            _healthAmount.SetMaxHealth(maxValue);
            _healthAmount.SetHealth(value);
        }
    }
}