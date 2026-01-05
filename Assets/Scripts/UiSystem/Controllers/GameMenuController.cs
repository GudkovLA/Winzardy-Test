#nullable enable

using Game.Common;
using UnityEngine;

namespace Game.UiSystem.Controllers
{
    public class GameMenuController : MonoBehaviour
    {
        [SerializeField]
        private GameOverMenuController _gameOverMenu = null!;

        public void InitializeFrom(ServiceLocator serviceLocator)
        {
            _gameOverMenu.InitializeFrom(serviceLocator);
        }
        
        public void StartGame()
        {
            gameObject.SetActive(false);
            _gameOverMenu.gameObject.SetActive(false);
        }

        public void ShowGameOverMenu()
        {
            if (_gameOverMenu.gameObject.activeSelf)
            {
                return;
            }
            
            gameObject.SetActive(true);
            _gameOverMenu.gameObject.SetActive(true);
        }
    }
}
