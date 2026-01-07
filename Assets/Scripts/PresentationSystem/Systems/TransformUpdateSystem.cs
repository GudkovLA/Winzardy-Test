#nullable enable

using Arch.Core;
using Game.Common.Components;
using Game.Common.Systems;
using Game.Common.Systems.Attributes;
using Game.PresentationSystem.Components;

namespace Game.PresentationSystem.Systems
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public class TransformUpdateSystem : AbstractSystem
    {
        private static readonly QueryDescription _transformQuery = new QueryDescription()
            .WithAll<Position, Rotation, InstanceLink>()
            .WithNone<Destroy>();

        protected override void OnUpdate()
        {
            World.Query(_transformQuery, 
                (ref Position position, ref Rotation rotation, ref InstanceLink instanceLink) =>
            {
                var transform = instanceLink.Instance.transform;
                transform.position = position.Value;
                transform.rotation = rotation.Value;
            });
        }
    }
}