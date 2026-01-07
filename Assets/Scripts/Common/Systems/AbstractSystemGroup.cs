#nullable enable

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Game.Common.Systems.Attributes;
using UnityEngine;
using UnityEngine.Pool;

namespace Game.Common.Systems
{
    public abstract class AbstractSystemGroup : AbstractSystem
    {
        private readonly List<AbstractSystem> _systems = new();

        public void AddSystem(AbstractSystem system)
        {
            _systems.Add(system);
        }

        public void UpdateOrder()
        {
            using var _ = ListPool<AbstractSystem>.Get(out var systemCache);
            systemCache.AddRange(_systems);

            for (var i = 0; i < systemCache.Count; i++)
            {
                var systemType = systemCache[i].GetType();
                var systemIndex = GetSystemIndex(systemType);

                var indexAfter = -1;
                var updateAfter = systemType.GetCustomAttribute<UpdateAfterAttribute>();
                if (updateAfter != null)
                {
                    indexAfter = GetSystemIndex(updateAfter.SystemType);

                    if (indexAfter >= 0)
                    {
                        systemIndex = MoveSystem(systemIndex, indexAfter + 1);
                        indexAfter = systemIndex - 1;
                    }
                    else
                    {
                        Debug.LogError(
                            $"Can't find system to order (SystemType={systemType.Name};UpdateAfter={updateAfter.SystemType})");
                    }
                }

                var updateBefore = systemType.GetCustomAttribute<UpdateBeforeAttribute>();
                if (updateBefore != null)
                {
                    var indexBefore = GetSystemIndex(updateBefore.SystemType);
                    if (indexBefore >= 0)
                    {
                        if (indexBefore < indexAfter)
                        {
                            Debug.LogError(
                                $"Can't order the system (SystemType={systemType.Name};UpdateBefore={updateBefore.SystemType})");
                        }
                        else if (indexBefore < systemIndex)
                        {
                            MoveSystem(systemIndex, indexBefore);
                        }
                    }
                    else
                    {
                        Debug.LogError(
                            $"Can't find the system to order (SystemType={systemType.Name};UpdateBefore={updateBefore.SystemType})");
                    }
                }
            }
        }

        public void LogStructure(StringBuilder output, string tab)
        {
            var subTab = $"{tab}  ";
            foreach (var system in _systems)
            {
                output.Append(tab).Append(system.GetType().Name).Append('\n');

                if (system is AbstractSystemGroup systemGroup)
                {
                    systemGroup.LogStructure(output, subTab);
                }
            }
        }

        protected override void OnDestroy()
        {
            foreach (var system in _systems)
            {
                system.Dispose();
            }

            _systems.Clear();

            base.OnDestroy();
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            foreach (var system in _systems)
            {
                system.Update();
            }
        }

        protected override void OnAfterUpdate()
        {
            base.OnAfterUpdate();

            Context.Playback(this);
        }

        private int GetSystemIndex(Type type)
        {
            for (var i = 0; i < _systems.Count; i++)
            {
                if (type == _systems[i].GetType())
                {
                    return i;
                }
            }

            return -1;
        }

        private int MoveSystem(int indexFrom, int indexTo)
        {
            var cache = _systems[indexFrom];
            if (indexTo > indexFrom)
            {
                _systems.Insert(indexTo, cache);
                _systems.RemoveAt(indexFrom);
                return indexTo - 1;
            }

            _systems.RemoveAt(indexFrom);
            _systems.Insert(indexTo, cache);
            return indexTo;
        }
    }
}