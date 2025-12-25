#nullable enable

using Arch.Core;
using Arch.Core.Extensions;

namespace Game.Common
{
    public readonly struct EntityHandle
    {
        public static EntityHandle Empty = new(Entity.Null);

        private readonly Entity _entity;
        private readonly int _version;

        public EntityHandle(Entity entity)
        {
            _entity = entity;
            _version = _entity.Version;
        }

        public bool IsValid()
        {
            return _entity != Entity.Null
                   && _version == _entity.Version
                   && _entity.IsAlive();
        }

        public Entity Value => _entity;
    }
}