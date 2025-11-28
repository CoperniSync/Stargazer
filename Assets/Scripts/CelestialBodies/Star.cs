using UnityEngine;
using ChargerAstronomyShared.Domain.Equatorial;
using ChargerAstronomyShared.Domain.Horizontal;
using ChargerAstronomyShared.Contracts.Models;
using System.Collections.Generic;

namespace Assets.Scripts.CelestialBodies
{
    /// <summary>
    /// A star that converts its celestial coordinates to a Unity position.
    /// Rendered by StarGpuRenderer (no per-star GameObject).
    /// </summary>
    public sealed class Star : CelestialBodyBase, IHorizontal
    {
        // Static registry so renderer can see all stars
        private static readonly List<Star> allStars = new List<Star>();
        public static IReadOnlyList<Star> AllStars => allStars;

        // Shared horizontal wrapper (provides Altitude, Azimuth, Magnitude, Distance)
        private HorizontalStar? horizontalStar;
        private HorizontalBody? horizontalBody;
        private EquatorialStar? equatorialStar;   // kept for id/name

        public HorizontalBody HorizontalBody => horizontalBody!;

        // Identity / display
        public int HipparcosId => horizontalStar?.HipparcosId ?? equatorialStar?.HipparcosId ?? 0;

        public string StarName =>
            string.IsNullOrWhiteSpace(horizontalStar?.StarName)
                ? (string.IsNullOrWhiteSpace(equatorialStar?.ProperName) ? "Unnamed Star" : equatorialStar!.ProperName!)
                : horizontalStar!.StarName!;

        public float Magnitude => (float)(horizontalBody?.Magnitude ?? equatorialStar?.Magnitude ?? 0.0);

        // cached positions
        public Vector3 Position3D { get; private set; }
        public Vector3 LocalScale { get; private set; }
        public Vector2 Position2D { get; private set; }

        // used by renderer
        public bool IsVisible { get; private set; } = false;

        /// <summary>
        /// Distance of the star sphere from origin.
        /// </summary>
        public float DrawnDistance { get; }

        public Star(HorizontalStar hstar, float drawnDistance = 24f)
        {
            horizontalStar = hstar;
            horizontalBody = hstar;
            DrawnDistance = drawnDistance;

            UpdateTransformFromHorizontal(); // sets Position3D, LocalScale, Position2D

            allStars.Add(this);
        }

        /// <summary>
        /// Backend push: apply a fresh HorizontalStar.
        /// </summary>
        public void ApplyHorizontal(HorizontalStar hstar)
        {
            horizontalStar = hstar;
            horizontalBody = hstar;
            UpdateTransformFromHorizontal();
        }

        /// <summary>
        /// Computes world position and updates screen-space cache.
        /// </summary>
        private void UpdateTransformFromHorizontal()
        {
            if (horizontalBody == null) return;

            Position3D = ComputeWorldPosition(
                (float)horizontalBody.Altitude,
                (float)horizontalBody.Azimuth,
                DrawnDistance
            );

            LocalScale = ComputeStarScale(Magnitude);

            var cam = Camera.main;
            if (cam != null)
            {
                var sp = cam.WorldToScreenPoint(Position3D);
                Position2D = new Vector2(sp.x, sp.y);
            }
        }

        /// <summary>
        /// X = r * cos(az) * cos(alt); Y = r * sin(alt); Z = r * cos(alt) * sin(az)
        /// (your original formula, kept as-is)
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

            // Standard: Az=0 (North) -> +Z, Az=90 (East) -> +X
            return new Vector3(
                radius * cosAlt * sinAz,    // X = East component
                radius * sinAlt,             // Y = Up (altitude)
                radius * cosAlt * cosAz      // Z = North component
            );
        }

        private static Vector3 ComputeStarScale(float magnitude)
        {
            float size = Mathf.Clamp(1.2f - 0.1f * (magnitude + 1.5f), 0.35f, 1.2f);
            return new Vector3(size, size, size);
        }

        public void ToggleState()
        {
            SetState(!IsVisible);
        }

        public void SetState(bool state)
        {
            IsVisible = state;
        }

        public void UpdatePosition()
        {
            UpdateTransformFromHorizontal();
        }
    }
}
