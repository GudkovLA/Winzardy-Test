#nullable enable

using System;
using Arch.Buffer;
using Arch.Core;

namespace Game.Common.Systems
{
    public abstract class AbstractSystem : IDisposable
    {
        private bool _initialized;
        private bool _disposed;

        protected SystemContext Context = null!;
        protected ServiceLocator ServiceLocator => Context.ServiceLocator;

        public World World => Context.World;
        public bool Disposed => _disposed;

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }
            
            OnDestroy();
            _disposed = true;
        }

        public void InitializeFrom(SystemContext systemContext)
        {
            Context = systemContext;
        }

        public void Update()
        {
            if (!_initialized)
            {
                OnCreate();
                _initialized = true;
            }

            OnBeforeUpdate();
            OnUpdate();
            OnAfterUpdate();
        }

        protected virtual void OnCreate()
        {
        }

        protected virtual void OnDestroy()
        {
        }

        protected virtual void OnBeforeUpdate()
        {
        }

        protected virtual void OnUpdate()
        {
        }

        protected virtual void OnAfterUpdate()
        {
        }

        protected CommandBuffer CreateCommandBuffer()
        {
            return new CommandBuffer();
        }

        protected CommandBuffer GetOrCreateCommandBuffer()
        {
            return Context.GetOrCreateCommandBuffer(this);
        }

        protected T? GetOrCreateSystem<T>() where T : AbstractSystem
        {
            return Context.GetOrCreateSystem<T>();
        }

        protected AbstractSystem? GetOrCreateSystem(Type type)
        {
            return Context.GetOrCreateSystem(type);
        }

        protected T? GetExistingSystem<T>() where T : AbstractSystem
        {
            return Context.GetExistingSystem<T>();
        }
    }
}