#nullable enable

using Game.Common;
using Game.Common.Events;
using Game.PresentationSystem;
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

        private EventsManager? _eventsManager;
        private GameWorld? _gameWorld;
        
        private void Awake()
        {
            _eventsManager = new EventsManager();
            _eventsManager.Subscribe<RestartGameEvent>(this, OnRestartGameEvent);
            
            var gameLevel = new GameLevel(_levelRoot, Vector3.zero, Quaternion.identity); 
            var gameCamera = new GameCamera(_camera);

            var instancePool = new InstancePool(_instancePoolRoot);
            var instanceFactory = new InstanceFactory(instancePool);
            InitializeInstancePool(instancePool);
            
            var gameInput = new GameInput();
            gameInput.Player.Enable();

            var canvas = _uiCanvas.gameObject.GetComponent<Canvas>();
            var gameUi = new GameUi(_camera, canvas);

            _gameWorld = new GameWorld(_gameSettings, 
                _eventsManager, 
                gameLevel, 
                gameCamera,
                gameInput,
                gameUi,
                instancePool,
                instanceFactory);

            _gameWorld.StartGame();
        }

        private void Update()
        {
            _eventsManager?.Update(Time.deltaTime);
            _gameWorld?.Update();
        }
        
        private void OnDestroy()
        {
            _gameWorld?.Dispose();
            _gameWorld = null;
        }

        private void OnRestartGameEvent(RestartGameEvent eventArgs)
        {
            _gameWorld?.RestartGame();
        }

        private void InitializeInstancePool(InstancePool instancePool)
        {
            _gameSettings.Prepare(instancePool);
        }
    }
}