#nullable enable

using System;
using Game.Common;
using Game.UiSystem.Controllers;
using Game.Utils;
using UnityEngine;

namespace Game
{
    public class GameUi : IDisposable
    {
        private Camera _camera;
        
        public Canvas Canvas { private set; get; }
        public HudController HudController { private set; get; }
        public GameMenuController GameMenuController { private set; get; }
        public Transform Root => Canvas.transform;

        public GameUi(Camera camera, Canvas canvas)
        {
            _camera = camera;
            Canvas = canvas;
            
            HudController = Canvas.GetComponentInChildren<HudController>(true);
            GameMenuController = Canvas.GetComponentInChildren<GameMenuController>(true);
        }

        public void InitializeFrom(ServiceLocator serviceLocator)
        {
            HudController.InitializeFrom(serviceLocator);
            GameMenuController.InitializeFrom(serviceLocator);
        }

        public void StartGame()
        {
            HudController.StartGame();
            GameMenuController.StartGame();
        }

        public Vector2 GetScreenPosition(Vector3 worldPosition)
        {
            var screenPoint = _camera.WorldToScreenPoint(worldPosition);            
            return new Vector2(screenPoint.x / Canvas.scaleFactor, screenPoint.y / Canvas.scaleFactor);
        }
        
        public void Dispose()
        {
        }
    }
}