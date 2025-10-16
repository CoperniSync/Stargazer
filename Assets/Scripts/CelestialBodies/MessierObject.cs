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
        private EquatorialMessierObject? equatorialMessier;

        // ----------------------------
        // Identification
        // ----------------------------
        public string MessierId => horizontalMessier?.MessierId ?? equatorialMessier?.MessierId ?? "Unknown M#";
        public string NewGeneralCatalog => horizontalMessier?.NewGeneralCatalog ?? equatorialMessier?.NewGeneralCatalog ?? "—";
        public string Type => horizontalMessier?.Type ?? equatorialMessier?.Type ?? "Unspecified";
        public string Constellation => horizontalMessier?.Constellation ?? equatorialMessier?.Constellation ?? "Unknown";
        public string Size => horizontalMessier?.Size ?? equatorialMessier?.Size ?? "—";
        public string ViewingSeason => horizontalMessier?.ViewingSeason ?? equatorialMessier?.ViewingSeason ?? "All Year";
        public string ViewingDifficulty => horizontalMessier?.ViewingDifficulty ?? equatorialMessier?.ViewingDifficulty ?? "Moderate";
        public string CommonName => horizontalMessier?.CommonName ?? equatorialMessier?.CommonName ?? "Unnamed Object";

        //  Positions for rendering
        public float Magnitude => (float)(horizontalBody?.Magnitude ?? equatorialMessier?.Magnitude ?? 6.0f);

        public Vector3 Position3D { get; private set; }
        public Vector2 Position2D { get; private set; }

        /// <summary>
        /// One-time initialization from horizontal data.
        /// </summary>
        public void FromHorizontal(HorizontalMessierObject hMessier, float drawnDistance = 125f)
        {
            horizontalMessier = hMessier;
            horizontalBody = hMessier;
            equatorialMessier = hMessier.EquatorialBody as EquatorialMessierObject;
            equatorialBody = hMessier.EquatorialBody;
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
            equatorialMessier = hMessier.EquatorialBody as EquatorialMessierObject;
            equatorialBody = hMessier.EquatorialBody;

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

            transform.position = Position3D;

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