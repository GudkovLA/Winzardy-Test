using Arch.Buffer;
using Arch.Core;

namespace Game.Common
{
    public readonly struct BuildContext
    {
        public readonly World World;
        public readonly CommandBuffer CommandBuffer;

        public BuildContext(World world, CommandBuffer commandBuffer)
        {
            World = world;
            CommandBuffer = commandBuffer;
        }
    }
    
    public interface IEntityBuilder
    {
        void Build(Entity entity, BuildContext context); 
    }
}