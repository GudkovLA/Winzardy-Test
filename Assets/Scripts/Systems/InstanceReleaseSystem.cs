#nullable enable

using Arch.Core;
using Game.Common.Systems;
using Game.Common.Systems.Attributes;
using Game.Components;
using Game.Utils;

namespace Game.Systems
{
    [UpdateInGroup(typeof(CleanupSystemGroup))]
    [UpdateBefore(typeof(EntityDestroySystem))]
    public class InstanceReleaseSystem : AbstractSystem
    {
        private static readonly QueryDescription _transformCleanupQuery = new QueryDescription()
            .WithAll<InstanceLink, PrefabId, Destroy>();

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

        protected override void OnUpdate()
        {
            World.Query(_transformCleanupQuery,
                (ref InstanceLink instanceLink, ref PrefabId prefabId) =>
                {
                    _instanceFactory.Destroy(prefabId.Value, instanceLink.Instance.gameObject);
                });
        }        
    }
}