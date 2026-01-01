#nullable enable 

using Arch.Buffer;
using Arch.Core;
using Game.Utils;

namespace Game.AbilitySystem
{
    public interface IAbility
    {
        void Prepare(InstancePool instancePool);
        bool CanActivate(World world, Entity ownerEntity);
        void Activate(World world, CommandBuffer commandBuffer, Entity ownerEntity);
    }
}