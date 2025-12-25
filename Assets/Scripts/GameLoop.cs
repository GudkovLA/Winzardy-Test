using Game.Settings;
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
        private Camera _camera;

        private GameWorld _gameWorld;
        
        private void Awake()
        {
            var gameLevel = new GameLevel(_levelRoot); 
            var gameCamera = new GameCamera(_camera);
            var gameInput = new GameInput();
            gameInput.Player.Enable();

            _gameWorld = new GameWorld(_gameSettings, 
                _characterSettings, 
                _enemySettings, 
                gameLevel, 
                gameCamera,
                gameInput);
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