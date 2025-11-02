using UnityEngine;
using ChargerAstronomyShared.Domain.Equatorial;
using ChargerAstronomyShared.Domain.Horizontal;
using NUnit.Framework;
using System.Collections.Generic;

namespace Assets.Scripts.CelestialBodies
{
    /// <summary>
    /// A star that converts its celestial coordinates to a Unity position.
    /// Author: Tommy Rodriguez
    /// Created: 2025-09-25
    /// </summary>
    public sealed class Star : CelestialBodyBase
    {

        // <summary
        // Constructor for queue initialization of star
        //</summary>

        private GameObject go;
        public Star(HorizontalStar hstar, float drawnDistance = 74f, GameObject starPrefab)
        {
            horizontalStar = hstar;
            horizontalBody = hstar;
            DrawnDistance = drawnDistance;

            UpdateTransformFromHorizontal(); // sets Position3D

            // spawn prefab
            go = Object.Instantiate(starPrefab, Position3D, Quaternion.identity);
            // Add scale computation later

            activeStars.Add(this);
        }

        // registry of spawned stars
        private static readonly List<Star> activeStars = new List<Star>();
        public static IReadOnlyList<Star> ActiveStars => activeStars;

        // Strongly typed identity from the catalog
        private EquatorialStar? equatorialStar;
        // Shared horizontal wrapper (provides Altitude, Azimuth, Magnitude, Distance)
        private HorizontalStar? horizontalStar;
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
        public Vector3 LocalScale { get; private set; }
        public Vector2 Position2D { get; private set; }

        /// <summary>);
        /// One-time initializer from a HorizontalStar snapshot.
        /// Call this when you first create the GameObject/component.
        /// </summary

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

            LocalScale = ComputeStarScale(Magnitude);

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
            float size = Mathf.Clamp(1.2f - 0.1f * (magnitude + 1.5f), 0.35f, 1.2f);
            return new Vector3(size, size, size);
        }

        /// <summary>
        /// Enable or disable the star's GameObject.
        /// </summary>
        public void ToggleState()
        {
            if (go != null)
            {
                go.SetActive(!go.activeSelf);
            }
        }

        /// <summary>
        /// Enable or disable the star's GameObject.
        /// </summary>
        public void UpdateStar(HorizontalStar hstar)
        {
            horizontalStar = hstar
            horizontalBody = hstar;

            UpdateTransformFromHorizontal();

            // update GameObject transform
            if (go != null)
            {
                go.transform.position = Position3D;
            }
        }

    }
}