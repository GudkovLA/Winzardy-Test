#nullable enable

using System;
using System.Collections.Generic;
using Arch.Buffer;
using Arch.Core;

namespace Game.Common.Systems
{

    public class SystemContext
    {
        private readonly Dictionary<AbstractSystemGroup, CommandBuffer> _commandBuffers = new();
        private readonly SystemManager _systemManager;
        private readonly WorldHandle _worldHandle;

        public ServiceLocator ServiceLocator { private set; get; }
        public float DeltaTime { private set; get; }
        public float Time { private set; get; }
        public World World => _worldHandle.Value;

        public SystemContext(SystemManager systemManager, WorldHandle worldHandle, ServiceLocator serviceLocator)
        {
            _systemManager = systemManager;
            _worldHandle = worldHandle;
            ServiceLocator = serviceLocator;
        }

        public void SetTime(float timeSinceStartup)
        {
            Time = timeSinceStartup;
        }

        public void SetDeltaTime(float deltaTime)
        {
            DeltaTime = deltaTime;
        }

        public CommandBuffer GetOrCreateCommandBuffer(AbstractSystem system)
        {
            var systemGroup = _systemManager.GetParentSystemGroup(system);
            if (systemGroup == null)
            {
                throw new NullReferenceException("System group can not be null");
            }

            if (!_commandBuffers.TryGetValue(systemGroup, out var commandBuffer))
            {
                commandBuffer = new CommandBuffer();
                _commandBuffers.Add(systemGroup, commandBuffer);
            }

            return commandBuffer;
        }

        public T? GetOrCreateSystem<T>() where T : AbstractSystem
        {
            return _systemManager.GetOrCreateSystem<T>();
        }

        public AbstractSystem? GetOrCreateSystem(Type type)
        {
            return _systemManager.GetOrCreateSystem(type);
        }

        public T? GetExistingSystem<T>() where T : AbstractSystem
        {
            return _systemManager.GetExistingSystem<T>();
        }

        public void Playback(AbstractSystemGroup systemGroup)
        {
            if (!_commandBuffers.Remove(systemGroup, out var commandBuffer))
            {
                return;
            }

            commandBuffer.Playback(_worldHandle.Value);
            commandBuffer.Dispose();
        }
    }
}