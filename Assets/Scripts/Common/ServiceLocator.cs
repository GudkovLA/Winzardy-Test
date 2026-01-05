#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Game.Common
{
    public class ServiceLocator : IDisposable
    {
        private readonly Dictionary<Type, IDisposable> _services = new();

        public void Dispose()
        {
            foreach (var entry in _services)
            {
                entry.Value.Dispose();
            }
            
            _services.Clear();
        }

        public void Register(IDisposable serviceInstance)
        {
            var type = serviceInstance.GetType();
            if (!_services.TryAdd(type, serviceInstance) && !Equals(_services[type], serviceInstance))
            {
                return;
            }
        }
        
        public void Register(Type type, IDisposable serviceInstance)
        {
            if (!_services.TryAdd(type, serviceInstance) && !Equals(_services[type], serviceInstance))
            {
                return;
            }
        }

        public T GetRequired<T>()
        {
            var result = (T?) _services.GetValueOrDefault(typeof(T));
            return result ?? throw new NullReferenceException($"Can't find service (ServicerType={typeof(T).Name})");
        }

        public bool TryGet<T>([NotNullWhen(true)] out T service)
        {
            if (!_services.TryGetValue(typeof(T), out var serviceInstance))
            {
                Debug.LogError($"Can't find service {typeof(T).Name}");
                service = default!;
                return false;
            }

            service = (T) serviceInstance;
            return true;
        }
    }
}