#nullable enable

using Arch.Core;
using Game.Common.Components;
using Game.Common.Systems;
using Game.Common.Systems.Attributes;
using Game.PresentationSystem.Components;
using Game.Systems;
using Game.Utils;

namespace Game.PresentationSystem.Systems
{
    [UpdateInGroup(typeof(CleanupSystemGroup))]
    [UpdateBefore(typeof(EntityDestroySystem))]
    public class InstanceReleaseSystem : AbstractSystem
    {
        private static readonly QueryDescription _transformDestroyQuery = new QueryDescription()
            .WithAll<InstanceLink, PrefabId, Destroy>();

        private static readonly QueryDescription _transformDestroyAllQuery = new QueryDescription()
            .WithAll<InstanceLink, PrefabId>();

        private InstanceFactory _instanceFactory = null!;
        private bool _initialized;

        protected override void OnCreate()
        {
            base.OnCreate();

            if (!ServiceLocator.TryGet(out _instanceFactory))
            {
                return;
            }

            _initialized = true;
        }

        protected override void OnDestroy()
        {
            World.Query(_transformDestroyAllQuery,
                (ref InstanceLink instanceLink, ref PrefabId prefabId) =>
                {
                    _instanceFactory.Destroy(prefabId.Value, instanceLink.Instance.gameObject);
                });

            base.OnDestroy();
        }        

        protected override void OnUpdate()
        {
            if (!_initialized)
            {
                return;
            }
            
            World.Query(_transformDestroyQuery,
                (ref InstanceLink instanceLink, ref PrefabId prefabId) =>
                {
                    _instanceFactory.Destroy(prefabId.Value, instanceLink.Instance.gameObject);
                });
        }        
    }
}