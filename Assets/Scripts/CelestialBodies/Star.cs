using UnityEngine;
using ChargerAstronomyShared.Domain.Equatorial;
using ChargerAstronomyShared.Domain.Horizontal;

namespace Assets.Scripts.CelestialBodies
{
    /// <summary>
    /// A star that converts its celestial coordinates to a Unity position.
    /// Author: Tommy Rodriguez
    /// Created: 2025-09-25
    /// Refactored: 2025-09-28
    /// </summary>
    public sealed class Star : CelestialBodyBase
    {
        // Strongly typed identity from the catalog
        private EquatorialStar? equatorialStar;

        // Shared horizontal wrapper (provides Altitude, Azimuth, Magnitude, Distance)
        private HorizontalStar? horizontalStar;

        // If your base also declares a HorizontalBody field, do NOT "new" it here.
        // Keep a local typed reference for star-specific fields.
        private HorizontalBody? horizontalBody;

        // Identity / display
        public int HipparcosId => horizontalStar?.HipparcosId ?? equatorialStar?.HipparcosId ?? 0;

        public string StarName =>
            string.IsNullOrWhiteSpace(horizontalStar?.StarName)
                ? (string.IsNullOrWhiteSpace(equatorialStar?.ProperName) ? "Unnamed Star" : equatorialStar!.ProperName!)
                : horizontalStar!.StarName!;

        // Photometric
        public float Magnitude => (float)(horizontalBody?.Magnitude ?? equatorialStar?.Magnitude ?? 0.0);

        // Cached positions
        public Vector3 Position3D { get; private set; }
        public Vector2 Position2D { get; private set; }

        /// <summary>
        /// One-time initializer from a HorizontalStar snapshot.
        /// Call this when you first create the GameObject/component.
        /// </summary>
        public void FromHorizontal(HorizontalStar hstar, float drawnDistance = 74f)
        {
            horizontalStar = hstar;
            horizontalBody = hstar;
            equatorialStar = hstar.EquatorialBody as EquatorialStar;
            equatorialBody = hstar.EquatorialBody;
            DrawnDistance = drawnDistance;

            UpdateTransformFromHorizontal();
        }

        /// <summary>
        /// Backend push: apply a fresh HorizontalStar.
        /// </summary>
        public void ApplyHorizontal(HorizontalStar hstar)
        {
            horizontalStar = hstar;
            horizontalBody = hstar;
            equatorialStar = hstar.EquatorialBody as EquatorialStar;
            equatorialBody = hstar.EquatorialBody;

            UpdateTransformFromHorizontal();
        }

        /// <summary>
        /// Computes world position and updates transform + screen-space cache.
        /// </summary>
        private void UpdateTransformFromHorizontal()
        {
            if (horizontalBody == null) return;

            // Convert Alt/Az (deg) -> world position on a sphere of radius DrawnDistance
            Position3D = ComputeWorldPosition(
                (float)horizontalBody.Altitude,
                (float)horizontalBody.Azimuth,
                DrawnDistance
            );

            transform.position = Position3D;

            // Scale by magnitude for a simple visual cue
            transform.localScale = ComputeStarScale(Magnitude);

            // Cache screen-space for UI
            var cam = Camera.main;
            if (cam != null)
            {
                var sp = cam.WorldToScreenPoint(Position3D);
                Position2D = new Vector2(sp.x, sp.y);
            }
        }

        /// <summary>
        /// Maps Altitude (deg), Azimuth (deg), and radius to a Unity world position.
        /// X = r * cos(az) * cos(alt); Y = r * sin(alt); Z = r * cos(alt) * sin(az)
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
                radius * (cosAz * cosAlt),
                radius * sinAlt,
                radius * (cosAlt * sinAz)
            );
        }

        /// <summary>
        /// Magnitude-to-scale mapping.
        /// </summary>
        private static Vector3 ComputeStarScale(float magnitude)
        {
            // Map mags roughly into [0.35, 1.2]
            // Example: mag -1.5 -> ~1.2; mag 6.0 -> ~0.35
            float size = Mathf.Clamp(1.2f - 0.1f * (magnitude + 1.5f), 0.35f, 1.2f);
            return new Vector3(size, size, size);
        }
    }
}