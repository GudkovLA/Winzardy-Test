using Game.Settings;
using Game.Utils;
using UnityEngine;

namespace Game
{
    public class GameLoop : MonoBehaviour
    {
        [SerializeField]
        private GameSettings _gameSettings;
        
        [SerializeField]
        private CharacterSettings _characterSettings;

        [SerializeField]
        // TODO: Add more types of enemies
        private EnemySettings _enemySettings;
        
        [SerializeField]
        private Transform _levelRoot;
        
        [SerializeField]
        private Transform _instancePoolRoot;
        
        [SerializeField]
        private Transform _uiCanvas;
        
        [SerializeField]
        private Camera _camera;

        private GameWorld _gameWorld;
        
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
            var gameUi = new GameUi(_camera, canvas);

            _gameWorld = new GameWorld(_gameSettings, 
                _characterSettings, 
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
            _gameWorld.Update();
        }
        
        private void OnDestroy()
        {
            _gameWorld.Dispose();
            _gameWorld = null;
        }

        private void InitializeInstancePool(InstancePool instancePool)
        {
            instancePool.Register(_characterSettings.Prefab, _characterSettings.PoolSize);
            // instancePool.Register(characterSettings.Projectile.Prefab, 20);
            instancePool.Register(_enemySettings.Prefab, _enemySettings.PoolSize);
            instancePool.Register(_enemySettings.CoinSettings.Prefab, _enemySettings.CoinSettings.PoolSize);
            instancePool.Register(_gameSettings.HealthViewPrefab, _gameSettings.HealthViewPoolSize);
        }
    }
}