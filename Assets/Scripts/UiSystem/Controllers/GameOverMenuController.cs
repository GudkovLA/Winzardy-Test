#nullable enable

using Game.Common;
using Game.Common.Events;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UiSystem.Controllers
{
    public class GameOverMenuController : MonoBehaviour
    {
        [SerializeField]
        private Button _startGameButton = null!;
        
        private EventsManager _eventsManager = null!;
        
        public void InitializeFrom(ServiceLocator serviceLocator)
        {
            _eventsManager = serviceLocator.GetRequired<EventsManager>();
        }
        
        private void Awake()
        {
            _startGameButton.onClick.AddListener(OnStartGamePressed);
        }
        
        private void OnDestroy()
        {
            _startGameButton.onClick.RemoveListener(OnStartGamePressed);
        }

        private void OnStartGamePressed()
        {
            _eventsManager.PublishEvent(new RestartGameEvent());
        }
    }
}
