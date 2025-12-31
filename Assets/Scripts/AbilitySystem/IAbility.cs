using Arch.Buffer;
using Arch.Core;
using Game.Utils;
using UnityEngine;

namespace Game.AbilitySystem
{
    public interface IAbility
    {
        void Prepare(InstancePool instancePool);
        void Activate(World world, CommandBuffer commandBuffer, Vector3 ownerPosition);
    }
}