using UnityEngine;
using ChargerAstronomyShared.Domain.Equatorial;
using ChargerAstronomyShared.Domain.Horizontal;

namespace Assets.Scripts.CelestialBodies
{
    /// <summary>
    /// Represents the Sun as a Unity object using horizontal coordinates.
    /// Author: Tommy Rodriguez
    /// Created: 2025-10-05
    /// </summary>
    public sealed class Sun : CelestialBodyBase
    {
        // spawned instance

        public HorizontalSun HorizontalSun => horizontalSun;
        private GameObject go;
        public GameObject Go { get => go; }

        private HorizontalSun? horizontalSun;
        private HorizontalBody? horizontalBody;

        // The Sun's brightness magnitude
        public float Magnitude => (float)(horizontalBody?.Magnitude);


        // Positions for rendering
        public Vector3 Position3D { get; private set; }
        public Vector3 LocalScale { get; private set; }
        public Vector2 Position2D { get; private set; }

        /// <summary>
        /// Constructor for single Sun instantiation.
        /// </summary>
        public Sun(HorizontalSun hsun, float drawnDistance = 74f)
        {

            horizontalSun = hsun;
            horizontalBody = hsun;
            DrawnDistance = drawnDistance;

            // Load prefab
            GameObject sunPrefab = Resources.Load<GameObject>("Prefabs/Sun");

            UpdateTransformFromHorizontal(); // sets Position3D/LocalScale/Position2D

            // spawn prefab
            go = Object.Instantiate(sunPrefab, Position3D, Quaternion.identity);
            go.name = "Sun";
            go.transform.localScale = new Vector3(5, 5, 5);

        }

        /// <summary>
        /// Backend push: apply a fresh HorizontalSun.
        /// </summary>
        public void UpdatePosition()
        {

            UpdateTransformFromHorizontal();

            if (go != null)
            {
                go.transform.position = Position3D;
            }
        }

        /// <summary>
        /// Show/hide the Sun GameObject.
        /// </summary>
        public void ToggleState()
        {
            if (go != null) go.SetActive(!go.activeSelf);
        }

        /// <summary>
        /// One-time initialization from horizontal data.
        /// </summary>
        public void FromHorizontal(HorizontalSun hSun, float drawnDistance = 100f)
        {
            horizontalSun = hSun;
            horizontalBody = hSun;
            DrawnDistance = drawnDistance;

            UpdateTransformFromHorizontal();
        }

        /// <summary>
        /// Applies updated horizontal coordinates from the Engine.
        /// </summary>
        public void ApplyHorizontal(HorizontalSun hSun)
        {
            horizontalSun = hSun;
            horizontalBody = hSun;

            UpdateTransformFromHorizontal();
        }

        /// <summary>
        /// Updates the Sun’s position and scale in world and screen space.
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


            // Screen-space for UI
            var cam = Camera.main;
            if (cam != null)
            {
                var sp = cam.WorldToScreenPoint(Position3D);
                Position2D = new Vector2(sp.x, sp.y);
            }
        }

        /// <summary>
        /// Converts Altitude/Azimuth to a Unity world position at a given radius.
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
                radius * cosAlt * sinAz,    // X = East component
                radius * sinAlt,             // Y = Up (altitude)
                radius * cosAlt * cosAz      // Z = North component
            );
        }


    }
}