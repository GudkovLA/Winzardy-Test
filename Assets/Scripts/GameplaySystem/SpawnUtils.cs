using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.GameplaySystem
{
    public enum SpawnAreaId
    {
        Undefined = 0,
        Top = 1,
        Bottom = 2,
        Left = 3,
        Right = 4,
        TopLeft = 5,
        TopRight = 6,
        BottomLeft = 7,
        BottomRight = 8
    }
    
    public static class SpawnUtils
    {
        public static void ResizeArea(Vector3[] areaCorners, float scale)
        {
            var center = Vector3.zero;
            foreach (var point in areaCorners)
            {
                center += point;
            }

            center /= areaCorners.Length;

            for (var i = 0; i < areaCorners.Length; i++)
            {
                var offset = areaCorners[i] - center;
                areaCorners[i] = center + offset * scale;
            }
        }

        public static Vector3 GetRandomPoint(Vector3[] areaCorners)
        {
            var factor = Random.value;
            var anchor = Random.value < 0.5f 
                ? 0 
                : 2;
            
            var p1 = Vector3.Lerp(areaCorners[anchor], areaCorners[1], factor);
            var p2 = Vector3.Lerp(areaCorners[anchor], areaCorners[3], factor);
            return Vector3.Lerp(p1, p2, Random.value);
        }
        
        public static void GetSpawnArea(
            CameraBoundaries cameraBoundaries, 
            SpawnAreaId spawnAreaId, 
            Vector3[] areaCorners, 
            float maxDepth)
        {
            switch (spawnAreaId)
            {
                case SpawnAreaId.Top:
                    GetTopArea(cameraBoundaries, areaCorners, maxDepth);
                    break;
                case SpawnAreaId.Bottom:
                    GetBottomArea(cameraBoundaries, areaCorners, maxDepth);
                    break;
                case SpawnAreaId.Left:
                    GetLeftArea(cameraBoundaries, areaCorners, maxDepth);
                    break;
                case SpawnAreaId.Right:
                    GetRightArea(cameraBoundaries, areaCorners, maxDepth);
                    break;
                case SpawnAreaId.TopLeft:
                    GetTopLeftArea(cameraBoundaries, areaCorners, maxDepth);
                    break;
                case SpawnAreaId.TopRight:
                    GetTopRightArea(cameraBoundaries, areaCorners, maxDepth);
                    break;
                case SpawnAreaId.BottomLeft:
                    GetBottomLeftArea(cameraBoundaries, areaCorners, maxDepth);
                    break;
                case SpawnAreaId.BottomRight:
                    GetBottomRightArea(cameraBoundaries, areaCorners, maxDepth);
                    break;
                default:
                    throw new NotImplementedException($"Spawn area is  not implemented (SpawnAreaId={spawnAreaId})");
            }
        }
        
        private static void GetTopArea(CameraBoundaries cameraBoundaries, Vector3[] areaCorners, float maxDepth)
        {
            areaCorners[0] = cameraBoundaries.FarLeft;
            areaCorners[1] = cameraBoundaries.FarRight;
            areaCorners[2] = cameraBoundaries.FarRight +
                             (cameraBoundaries.FarRight - cameraBoundaries.NearRight).normalized * maxDepth;
            areaCorners[3] = cameraBoundaries.FarLeft +
                             (cameraBoundaries.FarLeft - cameraBoundaries.NearLeft).normalized * maxDepth;
        }
        
        private static void GetBottomArea(CameraBoundaries cameraBoundaries, Vector3[] areaCorners, float maxDepth)
        {
            areaCorners[0] = cameraBoundaries.NearLeft;
            areaCorners[1] = cameraBoundaries.NearRight;
            areaCorners[2] = cameraBoundaries.NearRight -
                             (cameraBoundaries.FarRight - cameraBoundaries.NearRight).normalized * maxDepth;
            areaCorners[3] = cameraBoundaries.NearLeft -
                             (cameraBoundaries.FarLeft - cameraBoundaries.NearLeft).normalized * maxDepth;
        }

        private static void GetLeftArea(CameraBoundaries cameraBoundaries, Vector3[] areaCorners, float maxDepth)
        {
            areaCorners[0] = cameraBoundaries.NearLeft;
            areaCorners[1] = cameraBoundaries.FarLeft;
            areaCorners[2] = cameraBoundaries.FarLeft +
                             (cameraBoundaries.FarLeft - cameraBoundaries.FarRight).normalized * maxDepth;
            areaCorners[3] = cameraBoundaries.NearLeft +
                             (cameraBoundaries.NearLeft - cameraBoundaries.NearRight).normalized * maxDepth;
        }

        private static void GetRightArea(CameraBoundaries cameraBoundaries, Vector3[] areaCorners, float maxDepth)
        {
            areaCorners[0] = cameraBoundaries.NearRight;
            areaCorners[1] = cameraBoundaries.FarRight;
            areaCorners[2] = cameraBoundaries.FarRight +
                             (cameraBoundaries.FarRight - cameraBoundaries.FarLeft).normalized * maxDepth;
            areaCorners[3] = cameraBoundaries.NearRight +
                             (cameraBoundaries.NearRight - cameraBoundaries.NearLeft).normalized * maxDepth;
        }
        
        private static void GetTopLeftArea(CameraBoundaries cameraBoundaries, Vector3[] areaCorners, float maxDepth)
        {
            var upDir    = (cameraBoundaries.FarLeft - cameraBoundaries.NearLeft).normalized;
            var rightDir = (cameraBoundaries.NearRight - cameraBoundaries.NearLeft).normalized;
            
            var a = cameraBoundaries.FarLeft;
            var b = a - rightDir * maxDepth;

            areaCorners[0] = a;
            areaCorners[1] = b;
            areaCorners[2] = b + upDir * maxDepth;
            areaCorners[3] = a + upDir * maxDepth;
        }
        
        private static void GetTopRightArea(CameraBoundaries cameraBoundaries, Vector3[] areaCorners, float maxDepth)
        {
            var upDir    = (cameraBoundaries.FarRight - cameraBoundaries.NearRight).normalized;
            var rightDir = (cameraBoundaries.NearRight - cameraBoundaries.NearLeft).normalized;

            var a = cameraBoundaries.FarRight;
            var b = cameraBoundaries.FarRight + rightDir * maxDepth;

            areaCorners[0] = a;
            areaCorners[1] = a + upDir * maxDepth;
            areaCorners[2] = b + upDir * maxDepth;
            areaCorners[3] = b;
        }

        private static void GetBottomLeftArea(CameraBoundaries cameraBoundaries, Vector3[] areaCorners, float maxDepth)
        {
            var upDir    = (cameraBoundaries.FarLeft - cameraBoundaries.NearLeft).normalized;
            var rightDir = (cameraBoundaries.NearRight - cameraBoundaries.NearLeft).normalized;

            var a = cameraBoundaries.NearLeft - rightDir * maxDepth;
            var b = cameraBoundaries.NearLeft;

            areaCorners[0] = a;
            areaCorners[1] = b;
            areaCorners[2] = b - upDir * maxDepth;
            areaCorners[3] = a - upDir * maxDepth;
        }

        private static void GetBottomRightArea(CameraBoundaries cameraBoundaries, Vector3[] areaCorners, float maxDepth)
        {
            var upDir    = (cameraBoundaries.FarRight - cameraBoundaries.NearRight).normalized;
            var rightDir = (cameraBoundaries.NearRight - cameraBoundaries.NearLeft).normalized;

            var a = cameraBoundaries.NearRight;
            var b = cameraBoundaries.NearRight + rightDir * maxDepth;

            areaCorners[0] = a;
            areaCorners[1] = b;
            areaCorners[2] = b - upDir * maxDepth;
            areaCorners[3] = a - upDir * maxDepth;
        }
    }
}