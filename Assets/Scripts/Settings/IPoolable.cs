using Game.Utils;

namespace Game.Settings
{
    public interface IPoolable
    {
        void Prepare(InstancePool instancePool);
    }
}