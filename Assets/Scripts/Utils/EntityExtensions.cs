#nullable enable

using Arch.Buffer;
using Arch.Core;
using Arch.Core.Extensions;

namespace Game.Utils
{
    public static class EntityExtensions
    {
        public static void CopyComponentToEntityIfExists<T>(this Entity entity, Entity targetEntity) where T : struct
        {
            if (!entity.TryGet<T>(out var component))
            {
                return;
            }

            if (targetEntity.Has<T>())
            {
                targetEntity.Set(component);
                return;
            }
            
            targetEntity.Add(component);
        }

        public static void CopyComponentToEntityIfExists<T>(
            this Entity entity, 
            Entity targetEntity,
            CommandBuffer commandBuffer) where T : struct
        {
            if (entity.TryGet<T>(out var component))
            {
                commandBuffer.Add(targetEntity, component);
            }
        }
    }
}