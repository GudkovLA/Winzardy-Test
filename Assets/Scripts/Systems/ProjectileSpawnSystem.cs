#nullable enable

using Game.Common.Systems;
using Game.Common.Systems.Attributes;

namespace Game.Systems
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class ProjectileSpawnSystem : AbstractSystem
    {
        // TODO: Use projectile pool        
    }
}