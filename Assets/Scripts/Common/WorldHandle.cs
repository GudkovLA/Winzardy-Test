#nullable enable

using System;
using Arch.Core;

namespace Game.Common
{
    public class WorldHandle : IDisposable
    {
        public World Value { private set; get; }

        public WorldHandle(World world)
        {
            Value = world;
        }

        public void Set(World value)
        {
            Value = value;
        }

        public void Dispose()
        {
            Value.Dispose();
        }
    }
}