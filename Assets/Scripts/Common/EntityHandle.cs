#nullable enable

using System;
using Arch.Core;
using Arch.Core.Extensions;

namespace Game.Common
{
    public readonly struct EntityHandle : IEquatable<EntityHandle>
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
        
        public bool Equals(EntityHandle other)
        {
            return _entity.Id == other._entity.Id && _version == other._entity.Version;
        }
        
        public override int GetHashCode()
        {
            return ((17 * 23 + _entity.Id) * 23 + _entity.WorldId) * 23 + _version;
        }
    }
}