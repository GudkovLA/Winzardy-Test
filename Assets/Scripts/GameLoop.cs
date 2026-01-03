#nullable enable

using Game.CharacterSystem.Settings;
using Game.Settings;
using Game.Utils;
using UnityEngine;

namespace Game
{
    public class GameLoop : MonoBehaviour
    {
        [SerializeField]
        private GameSettings _gameSettings = null!;
        
        [SerializeField]
        private PlayerSettings _playerSettings = null!;

        [SerializeField]
        // TODO: Add more types of enemies
        private EnemySettings _enemySettings = null!;
        
        [SerializeField]
        private Transform _levelRoot = null!;
        
        [SerializeField]
        private Transform _instancePoolRoot = null!;
        
        [SerializeField]
        private Transform _uiCanvas = null!;
        
        [SerializeField]
        private Camera _camera = null!;

        private GameWorld? _gameWorld;
        
        private void Awake()
        {
            var gameLevel = new GameLevel(_levelRoot, Vector3.zero, Quaternion.identity); 
            var gameCamera = new GameCamera(_camera);

            var instancePool = new InstancePool(_instancePoolRoot);
            var instanceFactory = new InstanceFactory(instancePool);
            InitializeInstancePool(instancePool);
            
            var gameInput = new GameInput();
            gameInput.Player.Enable();

            var canvas = _uiCanvas.gameObject.GetComponent<Canvas>();
            var gameUi = new GameUi(_camera, canvas, instancePool);

            _gameWorld = new GameWorld(_gameSettings, 
                _playerSettings, 
                _enemySettings, 
                gameLevel, 
                gameCamera,
                gameInput,
                gameUi,
                instancePool,
                instanceFactory);
        }

        private void Update()
        {
            _gameWorld?.Update();
        }
        
        private void OnDestroy()
        {
            _gameWorld?.Dispose();
            _gameWorld = null;
        }

        private void InitializeInstancePool(InstancePool instancePool)
        {
            InitializeInstancePool(instancePool, _playerSettings.Character.Prefab, _playerSettings.PoolSize);
            InitializeInstancePool(instancePool, _enemySettings.Character.Prefab, _enemySettings.PoolSize);
            InitializeInstancePool(instancePool, _gameSettings.HealthViewPrefab, _gameSettings.HealthViewPoolSize);

            foreach (var lootData in _enemySettings.Loot)
            {
                InitializeInstancePool(instancePool, lootData.Resource.Prefab, lootData.PoolSize);
            }
        }

        private static void InitializeInstancePool(InstancePool instancePool, GameObject? prefab, int poolSize)
        {
            if (prefab != null)
            {
                instancePool.Register(prefab, poolSize);
            }
        }
    }
}