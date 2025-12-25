#nullable enable

using System;
using System.Collections.Generic;

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
        
        public T? Get<T>()
        {
            return (T?) _services.GetValueOrDefault(typeof(T));
        }
    }
}