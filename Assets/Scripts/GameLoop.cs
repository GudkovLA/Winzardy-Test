#nullable enable

using Game.Common;
using Game.Common.Events;
using Game.PresentationSystem;
using Game.ResourceSystem;
using UnityEngine;

namespace Game
{
    public class GameLoop : MonoBehaviour
    {
        [SerializeField]
        private GameSettings _gameSettings = null!;
        
        [SerializeField]
        private Transform _levelRoot = null!;
        
        [SerializeField]
        private Transform _instancePoolRoot = null!;
        
        [SerializeField]
        private Transform _uiCanvas = null!;
        
        [SerializeField]
        private Camera _camera = null!;

        private StateMachine<StateId>? _stateMachine;
        private GameWorld? _gameWorld;

        private ServiceLocator _serviceLocator = null!;
        private EventsManager _eventsManager = null!;
        private GameUi _gameUi = null!;
        
        private void Awake()
        {
            _stateMachine = new StateMachine<StateId>();
            _stateMachine.AddState(StateId.GameInProgress, 
                OnGameInProgressEnter, 
                OnGameInProgressUpdate, 
                OnGameInProgressLeave);
            _stateMachine.AddState(StateId.GameOver, 
                OnGameOverEnter, 
                OnGameOverUpdate, 
                OnGameOverLeave);
            
            _eventsManager = new EventsManager();
            _eventsManager.Subscribe<RestartGameEvent>(this, OnRestartGameEvent);

            var instancePool = new InstancePool(_instancePoolRoot);
            var instanceFactory = new InstanceFactory(instancePool);
            InitializeInstancePool(instancePool);

            var resourcesRegistry = new ResourcesRegistry();
            resourcesRegistry.CreateResources(_gameSettings);

            var gameInput = new GameInput();
            gameInput.Player.Enable();

            var gameCamera = new GameCamera(_camera);
            var gameLevel = new GameLevel(_levelRoot, Vector3.zero, Quaternion.identity); 

            var canvas = _uiCanvas.gameObject.GetComponent<Canvas>();
            _gameUi = new GameUi(_camera, canvas);
            
            _serviceLocator = new ServiceLocator();
            _serviceLocator.Register(_gameSettings);
            _serviceLocator.Register(_eventsManager);
            _serviceLocator.Register(instancePool);
            _serviceLocator.Register(instanceFactory);
            _serviceLocator.Register(resourcesRegistry);
            _serviceLocator.Register(gameInput);
            _serviceLocator.Register(gameCamera);
            _serviceLocator.Register(gameLevel);
            _serviceLocator.Register(_gameUi);

            _gameUi.InitializeFrom(_serviceLocator);
            _stateMachine?.SetState(StateId.GameInProgress);
        }

        private void Update()
        {
            _stateMachine?.Update();
        }
        
        private void OnDestroy()
        {
            _gameWorld?.Dispose();
            _stateMachine?.Dispose();
            _serviceLocator.Dispose();
        }

        private void OnRestartGameEvent(RestartGameEvent eventArgs)
        {
            _stateMachine?.SetState(StateId.GameInProgress);
        }

        private void InitializeInstancePool(InstancePool instancePool)
        {
            _gameSettings.Prepare(instancePool);
        }

        private void OnGameInProgressEnter()
        {
            _gameWorld = new GameWorld(_serviceLocator);
            _gameWorld.StartGame();
            _gameUi.StartGame();
        }
        
        private void OnGameInProgressUpdate()
        {
            _eventsManager.Update(Time.deltaTime);
            _gameWorld?.Update();

            if (_gameWorld?.IsGameOver() == true)
            {
                _stateMachine?.SetState(StateId.GameOver);
            }
        }

        private void OnGameInProgressLeave()
        {
        }
        
        private void OnGameOverEnter()
        {
            _gameUi.GameMenuController.ShowGameOverMenu();
        }
        
        private void OnGameOverUpdate()
        {
            _eventsManager.Update(Time.deltaTime);
            _gameWorld?.Update();
        }

        private void OnGameOverLeave()
        {
            _gameWorld?.Dispose();
            _gameWorld = null;
        }

        private enum StateId
        {
            GameInProgress,
            GameOver
        }
    }
}