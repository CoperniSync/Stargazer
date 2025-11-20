
using UnityEngine;
using ChargerAstronomyShared.Domain.Equatorial;
using ChargerAstronomyShared.Domain.Horizontal;

namespace Assets.Scripts.CelestialBodies
{
    /// <summary>
    /// Represents the Moon in Unity using horizontal coordinates.
    /// Author: Tommy Rodriguez
    /// Created: 2025-10-05
    /// </summary>
    public sealed class Moon : CelestialBodyBase
    {

        private GameObject go;

        public HorizontalMoon HorizontalMoon => horizontalMoon;
        private HorizontalMoon? horizontalMoon;
        private HorizontalBody? horizontalBody;


        // Moon's brightness magnitude
        public float Magnitude => (float)(horizontalBody?.Magnitude);

        // Phase angle of the Moon
        public float PhaseAngle => (float)(horizontalMoon?.Phase ?? 0.0);

        // Positions for rendering
        public Vector3 Position3D { get; private set; }
        public Vector3 LocalScale { get; private set; }
        public Vector2 Position2D { get; private set; }

        /// <summary>
        /// Constructor for single Moon instantiation.
        /// </summary>
        public Moon(HorizontalMoon hMoon, float drawnDistance = 95f)
        {
            horizontalMoon = hMoon;
            horizontalBody = hMoon;
            DrawnDistance = drawnDistance;

            UpdateTransformFromHorizontal();

            GameObject moonPrefab = Resources.Load<GameObject>("Prefabs/Moon");

            go = Object.Instantiate(moonPrefab, Position3D, Quaternion.identity);
            go.name = "Moon";
      
        }

        public void UpdatePosition()
        {
            UpdateTransformFromHorizontal();

            if (go != null)
            {
                go.transform.position = Position3D;
            }
        }

        /// <summary>
        /// One-time initialization from horizontal data.
        /// </summary>
        public void FromHorizontal(HorizontalMoon hMoon, float drawnDistance = 95f)
        {
            horizontalMoon = hMoon;
            horizontalBody = hMoon;
            DrawnDistance = drawnDistance;

            UpdateTransformFromHorizontal();
        }

        /// <summary>
        /// Applies updated horizontal coordinates from the Engine.
        /// </summary>
        public void ApplyHorizontal(HorizontalMoon hMoon)
        {
            horizontalMoon = hMoon;
            horizontalBody = hMoon;

            UpdateTransformFromHorizontal();
        }

        /// <summary>
        /// Updates the Sun’s position and scale in world and screen space.
        /// </summary>
        private void UpdateTransformFromHorizontal()
        {
            if (horizontalBody == null)
                return;

            // Convert Altitude/Azimuth to world coordinates
            Position3D = ComputeWorldPosition(
                (float)horizontalBody.Altitude,
                (float)horizontalBody.Azimuth,
                DrawnDistance
            );

            LocalScale = ComputeMoonScale(Magnitude, PhaseAngle);

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
                -(radius * (cosAz * cosAlt)),
                radius * sinAlt,
                radius * cosAlt * sinAz
            );
        }

        /// <summary>
        /// Computes a fixed but slightly dynamic Moon scale based on its phase.
        /// -- Can be dropped if we dont want a dynamic Moon or does cosine kitty handle this? -- Tommy
        /// </summary>
        private static Vector3 ComputeMoonScale(float magnitude, float phaseAngle)
        {
            // Base size scaling relative to brightness
            float size = Mathf.Clamp(1.2f - 0.05f * (magnitude + 12.7f), 0.5f, 1.4f);

            // Compute illumination fraction based on phase
            float illumination = 0.5f * (1 - Mathf.Cos(phaseAngle * Mathf.Deg2Rad)); // 0=new, 1=full
            float illuminatedScale = Mathf.Lerp(0.6f, 1.0f, illumination);

            size *= illuminatedScale;
            return new Vector3(size, size, size);
        }
    }
}
