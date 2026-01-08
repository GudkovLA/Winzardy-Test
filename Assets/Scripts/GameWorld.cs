#nullable enable

using System;
using System.Collections.Generic;
using System.Reflection;
using Arch.Core;
using Game.Common;
using Game.Common.Systems;
using Game.GameplaySystem.Components;
using Game.Utils;
using UnityEngine;

namespace Game
{
    public class GameWorld : IDisposable
    {
        private readonly QueryDescription _gameOverDescription = new QueryDescription()
            .WithAll<GameOverTag>();
        
        private readonly WorldHandle _worldHandle;
        private readonly ServiceLocator _serviceLocator;
        private SystemManager _systemManager = null!;

        public GameWorld(ServiceLocator serviceLocator)
        {
            _worldHandle = new WorldHandle(null!);
            _serviceLocator = serviceLocator;
        }

        public void Dispose()
        {
            _systemManager.Dispose();
            _worldHandle.Value.Dispose();
        }


        public void StartGame()
        {
            var world = World.Create();
            _worldHandle.Set(world);

            var assemblies =  GatherAssemblies();
            _systemManager = new SystemManager(_worldHandle, _serviceLocator);
            _systemManager.InitializeFrom(assemblies);
            _systemManager.LogStructure();
            
            world.CreatePlayerSingleton(_serviceLocator);
        }

        public void Update()
        {
            _systemManager.Update(Time.deltaTime, Time.realtimeSinceStartup);
        }
        
        public bool IsGameOver()
        {
            return _worldHandle.Value.CountEntities(_gameOverDescription) > 0;
        }
        
        private static List<Assembly> GatherAssemblies()
        {
            return new List<Assembly>
            {
                typeof(GameWorld).Assembly
            };
        }
    }
}