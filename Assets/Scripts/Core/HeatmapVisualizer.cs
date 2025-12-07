using UnityEngine;
using ChargerAstronomyEngine.Streaming;
using ChargerAstronomyShared.Domain.Index;
using ChargerAstronomyShared.Contracts.Models;
using System.Collections.Generic;
using ChargerAstronomyShared.Contracts.Repositories;

namespace Assets.Scripts.Core
{
    public class HeatMapVisualizer : MonoBehaviour
    {
        [Header("References")]
        public GameLoop gameLoop;

        [Header("Visualization Settings")]
        public bool showTiles = true;
        public bool showLabels = false;
        public float tileScale = 1.0f;
        public Color coldColor = new Color(0, 0, 1, 0.3f); // Blue, transparent
        public Color hotColor = new Color(1, 0, 0, 0.8f);  // Red, more opaque
        public Color activeTileColor = new Color(0, 1, 0, 0.5f); // Green for active tiles

        [Header("Camera Debug")]
        public bool showCameraDirection = true;
        public float cameraRayLength = 5f;
        public Color cameraRayColor = Color.yellow;

        [Header("Performance")]
        public bool showStats = true;

        private IEngineService<IHorizontal> engineService;
        private ITileIndex tileIndex;

        void Start()
        {
            if (gameLoop == null)
                gameLoop = FindObjectOfType<GameLoop>();
        }

        void OnDrawGizmos()
        {
            if (!showTiles || gameLoop == null) return;

            // trying to get engine service, very annoying
            if (engineService == null)
            {
                var field = gameLoop.GetType().GetField("engineService",
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Instance);
                if (field != null)
                    engineService = field.GetValue(gameLoop) as IEngineService<IHorizontal>;
            }

            if (engineService == null) return;

            if (tileIndex == null)
            {
                var spatialIndex = engineService.SpatialStarIndex;
                if (spatialIndex != null)
                    tileIndex = spatialIndex.TileIndex;
            }

            if (tileIndex == null) return;

            var heatServiceField = engineService.GetType().GetField("heatService",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

            var heatService = heatServiceField?.GetValue(engineService) as ChargerAstronomyEngine.Domain.Heat.HeatService;
            var heatMap = heatService?.heatMap;

            if (heatMap == null) return;

            foreach (var tuple in tileIndex.EnumerateGeometry())
            {
                var tileId = tuple.Item1;
                var geometry = tuple.Item2;

                float heat = heatMap.Get(tileId);

                Color tileColor = Color.Lerp(coldColor, hotColor, heat);

                Vector3 center = ToUnityVector(geometry.Center) * tileScale;

                Gizmos.color = tileColor;
                Gizmos.DrawSphere(center, 0.05f * tileScale);

                if (geometry.Vertices.Count >= 3)
                {
                    Vector3 v1 = ToUnityVector(geometry.Vertices[0]) * tileScale;
                    Vector3 v2 = ToUnityVector(geometry.Vertices[1]) * tileScale;
                    Vector3 v3 = ToUnityVector(geometry.Vertices[2]) * tileScale;

                    // draw triangle edges
                    Gizmos.color = tileColor;
                    Gizmos.DrawLine(v1, v2);
                    Gizmos.DrawLine(v2, v3);
                    Gizmos.DrawLine(v3, v1);

                    // if tile is hot, make it more visible
                    if (heat > 0.5f)
                    {
                        Gizmos.color = activeTileColor;
                        Gizmos.DrawLine(center, v1);
                        Gizmos.DrawLine(center, v2);
                        Gizmos.DrawLine(center, v3);
                    }
                }

                // draw labels if enabled
                if (showLabels && heat > 0.01f)
                {
#if UNITY_EDITOR

                    // this is a cool feature
                    UnityEditor.Handles.Label(center, $"T{tileId.Index}\nH:{heat:F2}");
#endif
                }
            }

            // draw camera direction
            if (showCameraDirection)
            {
                var cameraDir = Camera.main.transform.forward * cameraRayLength;
                Gizmos.color = cameraRayColor;
                Gizmos.DrawRay(Vector3.zero, cameraDir);
                Gizmos.DrawSphere(cameraDir, 0.1f);
            }
        }

        void OnGUI()
        {
            // always show something to test
            GUI.Box(new Rect(10, 10, 300, 50), "Heat Map Visualizer Active");

            if (engineService == null)
            {
                GUI.Label(new Rect(20, 35, 280, 20), "Engine Service: NOT FOUND");
                return;
            }

            GUI.Label(new Rect(20, 35, 280, 20), "Engine Service: FOUND");

            if (!showStats || engineService == null) return;

            // get stats if available
            try
            {
                if (engineService is EngineService<IHorizontal> engine)
                {
                    var stats = engine.GetStats();

                    GUI.Box(new Rect(10, 10, 300, 150), "Heat Map Debug");

                    GUI.Label(new Rect(20, 35, 280, 20), $"Total Tiles: {stats.TotalTiles}");
                    GUI.Label(new Rect(20, 55, 280, 20), $"Active Tiles: {stats.ActiveTiles}");
                    GUI.Label(new Rect(20, 75, 280, 20), $"Total Stars: {stats.TotalStars}");
                    GUI.Label(new Rect(20, 95, 280, 20), $"Active Stars: {stats.ActiveStars}");
                    GUI.Label(new Rect(20, 115, 280, 20), $"Update Queue: {stats.UpdateQueueSize}");
                    GUI.Label(new Rect(20, 135, 280, 20), $"FPS: {1f / Time.deltaTime:F1}");
                }
            }
            catch { }
        }

        // convert vector3 types
        private Vector3 ToUnityVector(System.Numerics.Vector3 v)
        {
            return new Vector3(v.X, v.Z, v.Y); // have to swap y and z bcz unity coordinate system
        }
    }
}