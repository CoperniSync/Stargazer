using UnityEngine;
using ChargerAstronomyShared.Domain.Equatorial;
using ChargerAstronomyShared.Domain.Horizontal;

namespace Assets.Scripts.CelestialBodies
{
    /// <summary>
    /// Represents a Messier deep-sky object 
    /// in Unity using horizontal coordinates.
    /// Author: Tommy Rodriguez
    /// Created: 2025-10-05
    /// </summary>
    public sealed class MessierObject : CelestialBodyBase
    {
        private HorizontalMessierObject? horizontalMessier;
        private HorizontalBody? horizontalBody;

        // ----------------------------
        // Identification
        // ----------------------------
        public string MessierId => horizontalMessier?.MessierId ?? "Unknown M#";
        public string NewGeneralCatalog => horizontalMessier?.NewGeneralCatalog ?? "—";
        public string Type => horizontalMessier?.Type ??  "Unspecified";
        public string Constellation => horizontalMessier?.Constellation ??  "Unknown";
        public string Size => horizontalMessier?.Size ??  "—";
        public string ViewingSeason => horizontalMessier?.ViewingSeason ?? "All Year";
        public string ViewingDifficulty => horizontalMessier?.ViewingDifficulty ??  "Moderate";
        public string CommonName => horizontalMessier?.CommonName ??  "Unnamed Object";

        //  Positions for rendering
        public float Magnitude => (float)(horizontalBody?.Magnitude);

        public Vector3 Position3D { get; private set; }
        public Vector2 Position2D { get; private set; }

        private GameObject go;

        /// <summary>
        /// Prefab-spawning constructor for Messier objects.
        /// </summary>
        public MessierObject(HorizontalMessierObject hMessier, float drawnDistance = 125f, GameObject prefab = null)
        {
            horizontalMessier = hMessier;
            horizontalBody = hMessier;
            DrawnDistance = drawnDistance;

            UpdateTransformFromHorizontal();

            GameObject messierObjectPrefab = Resources.Load<GameObject>("Prefabs/MessierObject");

            go = Object.Instantiate(messierObjectPrefab, Position3D, Quaternion.identity);
            go.name = $"Messier_{MessierId}";
        }

        /// <summary>
        /// Updates an existing Messier object's position and scale.
        /// </summary>
        public void UpdateMessier()
        {

            UpdateTransformFromHorizontal();

            if (go != null)
            {
                go.transform.position = Position3D;
            }
        }

        /// <summary>
        /// Enable or disable the Messier object GameObject.
        /// </summary>
        public void ToggleState()
        {
            if (go != null)
            {
                go.SetActive(!go.activeSelf);
            }
        }

        /// <summary>
        /// One-time initialization from horizontal data.
        /// </summary>
        public void FromHorizontal(HorizontalMessierObject hMessier, float drawnDistance = 125f)
        {
            horizontalMessier = hMessier;
            horizontalBody = hMessier;
            DrawnDistance = drawnDistance;

            UpdateTransformFromHorizontal();
        }

        /// <summary>
        /// Applies updated coordinates or metadata from Engine.
        /// </summary>
        public void ApplyHorizontal(HorizontalMessierObject hMessier)
        {
            horizontalMessier = hMessier;
            horizontalBody = hMessier;
            UpdateTransformFromHorizontal();
        }

        /// <summary>
        /// Converts horizontal coordinates to Unity world position and screen projection.
        /// </summary>
        private void UpdateTransformFromHorizontal()
        {
            if (horizontalBody == null)
                return;

            Position3D = ComputeWorldPosition(
                (float)horizontalBody.Altitude,
                (float)horizontalBody.Azimuth,
                DrawnDistance
            );

            // Screen-space position
            var cam = Camera.main;
            if (cam != null)
            {
                var sp = cam.WorldToScreenPoint(Position3D);
                Position2D = new Vector2(sp.x, sp.y);
            }
        }

        /// <summary>
        /// Converts Altitude/Azimuth to Unity world coordinates.
        /// </summary>
        private static Vector3 ComputeWorldPosition(float altitudeDeg, float azimuthDeg, float radius)
        {
            const float Deg2Rad = Mathf.PI / 180f;
            float alt = altitudeDeg * Deg2Rad;
            float az = azimuthDeg * Deg2Rad;

            float cosAlt = Mathf.Cos(alt);
            float sinAlt = Mathf.Sin(alt);
            float cosAz = Mathf.Cos(az);
            float sinAz = Mathf.Sin(az);

            return new Vector3(
                radius * cosAz * cosAlt,
                radius * sinAlt,
                radius * cosAlt * sinAz
            );
        }

    }
}