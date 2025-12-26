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
        private Camera _camera;

        private GameWorld _gameWorld;
        
        private void Awake()
        {
            var gameLevel = new GameLevel(_levelRoot, Vector3.zero, Quaternion.identity); 
            var gameCamera = new GameCamera(_camera);

            var instancePool = new InstancePool(_instancePoolRoot);
            instancePool.Register(_characterSettings.Prefab.GetInstanceID(), _characterSettings.Prefab, 1);
            instancePool.Register(_characterSettings.Projectile.Prefab.GetInstanceID(), _characterSettings.Projectile.Prefab, 20);
            instancePool.Register(_enemySettings.Prefab.GetInstanceID(), _enemySettings.Prefab, 30);
            var instanceFactory = new InstanceFactory(instancePool);
            
            var gameInput = new GameInput();
            gameInput.Player.Enable();

            _gameWorld = new GameWorld(_gameSettings, 
                _characterSettings, 
                _enemySettings, 
                gameLevel, 
                gameCamera,
                gameInput,
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
    }
}