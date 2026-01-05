#nullable enable

using System;
using System.Collections.Generic;
using System.Reflection;
using Arch.Core;
using Game.CharacterSystem.Settings;
using Game.Common;
using Game.Common.Systems;
using Game.ResourceSystem;
using Game.Settings;
using Game.Utils;
using UnityEngine;

namespace Game
{
    public class GameWorld : IDisposable
    {
        private readonly WorldHandle _worldHandle;
        private readonly ServiceLocator _serviceLocator;
        private readonly GameUi _gameUi;
        
        
        private SystemManager _systemManager = null!;

        public GameWorld(
            GameSettings gameSettings, 
            PlayerSettings playerSettings, 
            EnemySettings enemySettings,
            EventsManager eventsManager,
            GameLevel gameLevel,
            GameCamera gameCamera,
            GameInput gameInput,
            GameUi gameUi,
            InstancePool instancePool,
            InstanceFactory instanceFactory)
        {
            _worldHandle = new WorldHandle(null!);
            _gameUi = gameUi;

            var resourceManager = new ResourcesManager();
            resourceManager.CreateResources(enemySettings);

            _serviceLocator = new ServiceLocator();
            _serviceLocator.Register(gameSettings);
            _serviceLocator.Register(playerSettings);
            _serviceLocator.Register(enemySettings);
            _serviceLocator.Register(gameLevel);
            _serviceLocator.Register(gameCamera);
            _serviceLocator.Register(gameInput);
            _serviceLocator.Register(gameUi);
            _serviceLocator.Register(instancePool);
            _serviceLocator.Register(instanceFactory);
            _serviceLocator.Register(resourceManager);
            _serviceLocator.Register(eventsManager);
            
            _gameUi.InitializeFrom(_serviceLocator);
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

            _gameUi.StartGame();
        }

        public void RestartGame()
        {
            _systemManager.Dispose();

            StartGame();
        }

        public void Dispose()
        {
            _worldHandle.Value.Dispose();
            _serviceLocator.Dispose();
        }

        public void Update()
        {
            _systemManager.Update(Time.deltaTime, Time.realtimeSinceStartup);
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