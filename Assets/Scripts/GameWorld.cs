#nullable enable

using System;
using System.Collections.Generic;
using System.Reflection;
using Arch.Core;
using Game.Common;
using Game.Common.Systems;
using Game.Settings;
using UnityEngine;

namespace Game
{
    public class GameWorld : IDisposable
    {
        private readonly WorldHandle _worldHandle;
        private readonly SystemManager _systemManager;
        private readonly ServiceLocator _serviceLocator;

        public GameWorld(
            GameSettings gameSettings, 
            CharacterSettings characterSettings, 
            EnemySettings enemySettings,
            GameLevel gameLevel,
            GameCamera gameCamera,
            GameInput gameInput)
        {
            var world = World.Create();
            _worldHandle = new WorldHandle(world);

            _serviceLocator = new ServiceLocator();
            _serviceLocator.Register(gameSettings);
            _serviceLocator.Register(characterSettings);
            _serviceLocator.Register(enemySettings);
            _serviceLocator.Register(gameLevel);
            _serviceLocator.Register(gameCamera);
            _serviceLocator.Register(gameInput);
            
            _systemManager = new SystemManager(_worldHandle, _serviceLocator);
            _serviceLocator.Register(_systemManager);

            _systemManager.InitializeFrom(GatherAssemblies());
            _systemManager.LogStructure();
        }

        public void Dispose()
        {
            _worldHandle.Value.Dispose();
            _serviceLocator.Dispose();
        }

        public void Update()
        {
            _systemManager.Update(Time.deltaTime);
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