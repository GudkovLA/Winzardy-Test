#nullable enable

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Game.Common.Systems.Attributes;
using UnityEngine;

namespace Game.Common.Systems
{
    public class SystemManager : IDisposable
    {
        private readonly Dictionary<Type, AbstractSystem> _allSystems = new();
        private readonly Dictionary<Type, AbstractSystemGroup> _allGroups = new();
        private readonly Dictionary<AbstractSystem, AbstractSystemGroup> _parentGroups = new();
        private readonly LoopSystemGroup _loopSystemGroup = new();

        private readonly SystemContext _systemContext;

        public SystemManager(WorldHandle worldHandle, ServiceLocator serviceLocator)
        {
            _systemContext = new SystemContext(this, worldHandle, serviceLocator);

            _loopSystemGroup.InitializeFrom(_systemContext);
            _allGroups.Add(typeof(LoopSystemGroup), _loopSystemGroup);
        }

        public void InitializeFrom(List<Assembly> assemblies)
        {
            /*
            var selfAssembly = typeof(LoopSystemGroup).Assembly;
            if (!assemblies.Contains(selfAssembly))
            {
                CreateSystems(selfAssembly);
            }
            */
            foreach (var assembly in assemblies)
            {
                CreateSystemGroups(assembly);
            }
            
            GroupSystemGroups();

            /*
            if (!assemblies.Contains(selfAssembly))
            {
                CreateSystems(selfAssembly);
            }
            */
            
            foreach (var assembly in assemblies)
            {
                CreateSystems(assembly);
            }

            GroupSystems();
        }

        public void Dispose()
        {
            _loopSystemGroup.Dispose();
        }

        public void Update(float deltaTime)
        {
            _systemContext.SetDeltaTime(deltaTime);
            _loopSystemGroup.Update();
        }

        public T? GetOrCreateSystem<T>() where T : AbstractSystem
        {
            var type = typeof(T);
            if (!typeof(AbstractSystem).IsAssignableFrom(type))
            {
                return null;
            }

            var system = Activator.CreateInstance(type) as AbstractSystem;
            if (system == null)
            {
                Debug.LogError($"Can't create system of type (Type={type.Name})");
                return null;
            }

            AddSystemInternal(type, system);
            GroupSystem(type, system);
            return (T)system;
        }

        public AbstractSystem? GetOrCreateSystem(Type type)
        {
            if (!typeof(AbstractSystem).IsAssignableFrom(type))
            {
                return null;
            }

            var system = Activator.CreateInstance(type) as AbstractSystem;
            if (system == null)
            {
                Debug.LogError($"Can't create system of type (Type={type.Name})");
                return null;
            }

            AddSystemInternal(type, system);
            GroupSystem(type, system);
            return system;
        }

        public T? GetExistingSystem<T>() where T : AbstractSystem
        {
            return (T?)_allSystems.GetValueOrDefault(typeof(T));
        }

        public AbstractSystemGroup? GetParentSystemGroup(AbstractSystem system)
        {
            return _parentGroups.GetValueOrDefault(system);
        }

        public void LogStructure()
        {
            var output = new StringBuilder("\n").Append(_loopSystemGroup.GetType().Name).Append('\n');
            _loopSystemGroup.LogStructure(output, "  ");

            Debug.Log(output.ToString());
        }

        private void CreateSystemGroups(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (type.IsAbstract
                    || !typeof(AbstractSystemGroup).IsAssignableFrom(type)
                    || type.GetCustomAttribute<DisableAutoCreationAttribute>() != null)
                {
                    continue;
                }

                var system = Activator.CreateInstance(type) as AbstractSystemGroup;
                if (system == null)
                {
                    Debug.LogError($"Can't create system of type (Type={type.Name})");
                    continue;
                }

                AddSystemGroupInternal(type, system);
            }
        }

        private void CreateSystems(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (type.IsAbstract
                    || !typeof(AbstractSystem).IsAssignableFrom(type)
                    || typeof(AbstractSystemGroup).IsAssignableFrom(type)
                    || type.GetCustomAttribute<DisableAutoCreationAttribute>() != null)
                {
                    continue;
                }

                var system = Activator.CreateInstance(type) as AbstractSystem;
                if (system == null)
                {
                    Debug.LogError($"Can't create system of type (Type={type.Name})");
                    continue;
                }

                AddSystemInternal(type, system);
            }
        }

        private void AddSystemGroupInternal(Type type, AbstractSystemGroup systemGroup)
        {
            systemGroup.InitializeFrom(_systemContext);
            _allGroups.Add(type, systemGroup);
        }

        private void AddSystemInternal(Type type, AbstractSystem system)
        {
            system.InitializeFrom(_systemContext);
            _allSystems.Add(type, system);
        }

        private void GroupSystemGroups()
        {
            foreach (var entry in _allGroups)
            {
                var updateInGroup = entry.Key.GetCustomAttribute<UpdateInGroupAttribute>();
                if (updateInGroup == null)
                {
                    if (entry.Value != _loopSystemGroup)
                    {
                        _loopSystemGroup.AddSystem(entry.Value);
                        _parentGroups.Add(entry.Value, _loopSystemGroup);
                    }

                    continue;
                }

                if (!_allGroups.TryGetValue(updateInGroup.SystemType, out var systemGroup))
                {
                    Debug.LogError($"Can't find system group (Type={updateInGroup.SystemType.Name})");
                    continue;
                }

                systemGroup.AddSystem(entry.Value);
                _parentGroups.Add(entry.Value, systemGroup);
            }
        }

        private void GroupSystems()
        {
            foreach (var entry in _allSystems)
            {
                GroupSystem(entry.Key, entry.Value);
            }
        }

        private void GroupSystem(Type systemType, AbstractSystem system)
        {
            var updateInGroup = systemType.GetCustomAttribute<UpdateInGroupAttribute>();
            if (updateInGroup == null)
            {
                Debug.LogError($"Can't order system group of type (Type={systemType.Name})");
                return;
            }

            if (!_allGroups.TryGetValue(updateInGroup.SystemType, out var systemGroup))
            {
                Debug.LogError($"Can't find system group (Type={updateInGroup.SystemType.Name})");
                return;
            }

            systemGroup.AddSystem(system);
            _parentGroups.Add(system, systemGroup);
        }
    }
}