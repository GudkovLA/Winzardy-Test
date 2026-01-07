#nullable enable

using Arch.Core;
using Game.CharacterSystem;
using Game.CharacterSystem.Settings;
using Game.Common;
using Game.Common.Systems;
using Game.Common.Systems.Attributes;
using Game.ResourceSystem;
using Game.ResourceSystem.Components;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.SpawnSystem.Systems
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class EnemySpawnSystem : AbstractSystem
    {
        private static readonly CameraBoundaries _cameraBoundaries = new();
        private static readonly Vector3[] _areaPoints = new Vector3[4];
        private static readonly Collider[] _collidersCache = new Collider[4];

        private GameSettings _gameSettings = null!;
        private GameLevel _gameLevel = null!;
        private GameCamera _gameCamera = null!;
        private ResourcesManager _resourcesManager = null!;

        private float[] _weights;
        private SpawnAreaId[] _areasIds;
        private float[] _timeCounter;
        
        private bool _initialized;

        protected override void OnCreate()
        {
            base.OnCreate();

            if (!ServiceLocator.TryGet(out _gameSettings)
                || !ServiceLocator.TryGet(out _gameLevel)
                || !ServiceLocator.TryGet(out _gameCamera)
                || !ServiceLocator.TryGet(out _resourcesManager))
            {
                return;
            }
            
            _initialized = InitializeSpawnTimers() && InitializeSpawnAreas();
        }

        protected override void OnUpdate()
        {
            if (!_initialized)
            {
                return;
            }

            var world = Context.World;
            var groundPlane = _gameLevel.GroundPlane;
            var spawnSettings = _gameSettings.Spawn;

#if UNITY_EDITOR
            if (spawnSettings.DebugEnable)
            {
                DebugSpawnArea(groundPlane, spawnSettings.CameraAreaScale, spawnSettings.AreaMaxDepth);
            }
#endif

            for (var i = 0; i < _timeCounter.Length; i++)
            {
                var enemySpawnSettings = _gameSettings.Enemies[i];
                _timeCounter[i] -= Time.deltaTime;
                
                if (_timeCounter[i] < 0)
                {
                    TrySpawnEnemy(world, enemySpawnSettings.EnemySettings, spawnSettings, groundPlane);
                    _timeCounter[i] += enemySpawnSettings.SpawnTimeout;
                }
            }
        }

        private bool InitializeSpawnTimers()
        {
            if (_gameSettings.Enemies.Length == 0)
            {
                Debug.LogError($"Enemies list is empty");
                return false;
            }

            _timeCounter = new float[_gameSettings.Enemies.Length];  
            for (var i = 0; i < _gameSettings.Enemies.Length; i++)
            {
                _timeCounter[i] = _gameSettings.Enemies[i].FirstSpawnTimeout;
            }

            return true;
        }
        
        private bool InitializeSpawnAreas()
        {
            var spawnSettings = _gameSettings.Spawn;
            if (spawnSettings.SpawnAreas.Length == 0)
            {
                Debug.LogError($"Spawn areas list is empty");
                return false;
            }
            
            var summaryWeight = 0f;
            _areasIds = new SpawnAreaId[spawnSettings.SpawnAreas.Length];
            _weights = new float[spawnSettings.SpawnAreas.Length];
 
            for (var i = 0; i < spawnSettings.SpawnAreas.Length; i++)
            {
                _areasIds[i] = spawnSettings.SpawnAreas[i].AreaId;  
                _weights[i] = spawnSettings.SpawnAreas[i].Weight;
                summaryWeight += spawnSettings.SpawnAreas[i].Weight;
            }

            // Normalize weights
            var offsetWeight = 0f;
            for (var i = 0; i < spawnSettings.SpawnAreas.Length; i++)
            {
                _weights[i] = offsetWeight + _weights[i] / summaryWeight;
                offsetWeight = _weights[i];
            }

            return true;
        }

        private void TrySpawnEnemy(
            World world,
            EnemySettings enemySettings, 
            GameSettings.SpawnSettingsData spawnSettings, 
            Plane groundPlane)
        {
            if (enemySettings.Prefab == null)
            {
                Debug.LogError($"Character prefab is not defined");
                return;
            }

            _gameCamera.UpdateCameraBoundaries(groundPlane, _cameraBoundaries);
            SpawnUtils.ResizeArea(_cameraBoundaries.Corners, spawnSettings.CameraAreaScale);

            var triesCount = 0;
            var result = false;
            var spawnPosition = Vector3.zero;
            while (!result && triesCount < spawnSettings.SpawnRetryCountLimit)
            {
                triesCount += 1;
                result = TryGetSpawnPoint(spawnSettings.AreaMaxDepth, 
                    spawnSettings.SpawnCollisionSafeRadius,
                    out spawnPosition);
            }
            
            if (!result)
            {
                Debug.LogError($"Can't find point for enemy spawn");
                return;
            }

            var commandBuffer = Context.GetOrCreateCommandBuffer(this);
            var entity = CharacterUtils.SpawnCharacter(enemySettings,
                world,
                commandBuffer,
                spawnPosition,
                Quaternion.identity);

            if (entity == Entity.Null)
            {
                return;
            }

            if (_resourcesManager.TryGetDroppedResource(enemySettings.GetInstanceID(), out var resourceId))
            {
                commandBuffer.Add(entity, new ResourceSpawner { ResourceId = resourceId });
            }
        }

        private bool TryGetSpawnPoint(float areaMaxDepth, float collisionSafeRadius, out Vector3 spawnPoint)
        {
            var spawnAreaType = GetRandomSpawnArea();
            if (spawnAreaType == SpawnAreaId.Undefined)
            {
                Debug.LogError($"Spawn area type is not defined");
                spawnPoint = Vector3.zero;
                return false;
            }

            SpawnUtils.GetSpawnArea(_cameraBoundaries, spawnAreaType, _areaPoints, areaMaxDepth);

            // Check that point is not inside any obstacle collider
            spawnPoint = SpawnUtils.GetRandomPoint(_areaPoints);
            var count = Physics.OverlapSphereNonAlloc(spawnPoint, 
                collisionSafeRadius, 
                _collidersCache,
                (int) PhysicsLayer.Obstacle);

            return count == 0;
        }

        private SpawnAreaId GetRandomSpawnArea()
        {
            if (_weights.Length == 0)
            {
                return SpawnAreaId.Undefined;
            }
            
            var value = Random.value;
            for (var i = 0; i < _weights.Length; i++)
            {
                if (value < _weights[i])
                {
                    return _areasIds[i];
                }
            }
            
            return SpawnAreaId.Undefined;
        }

#if UNITY_EDITOR
        private void DebugSpawnArea(Plane groundPlane, float cameraAreaScale, float maxAreaDepth)
        {
            _gameCamera.UpdateCameraBoundaries(groundPlane, _cameraBoundaries);
            DrawArea(_cameraBoundaries.Corners, Color.green);

            SpawnUtils.ResizeArea(_cameraBoundaries.Corners, cameraAreaScale);
            DrawArea(_cameraBoundaries.Corners, Color.red);

            for (var i = 1; i <= 8; i++)
            {
                SpawnUtils.GetSpawnArea(_cameraBoundaries, (SpawnAreaId) i, _areaPoints, maxAreaDepth);
                DrawArea(_areaPoints, Color.yellow);

                for (var j = 0; j < 100; j++)
                {
                    var point = SpawnUtils.GetRandomPoint(_areaPoints);
                    Debug.DrawRay(point, Vector3.up * 0.3f, Color.magenta, 1f);
                }
            }
        }

        private static void DrawArea(Vector3[] points, Color color)
        {
            for (var i = 0; i <= points.Length; i++)
            {
                Debug.DrawLine(points[i % 4], points[(i + 1) % 4], color);
            }
        }
#endif        
    }
}
