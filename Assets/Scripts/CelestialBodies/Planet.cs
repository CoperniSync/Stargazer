using UnityEngine;
using ChargerAstronomyShared.Domain.Equatorial;
using ChargerAstronomyShared.Domain.Horizontal;

namespace Assets.Scripts.CelestialBodies
{
    /// <summary>
    /// A planet that converts its celestial coordinates to a Unity position.
    /// Author: Tommy Rodriguez
    /// Created: 2025-10-05
    /// </summary>
    public sealed class Planet : CelestialBodyBase
    {
        private GameObject go;

        private HorizontalPlanet? horizontalPlanet;
        private HorizontalBody? horizontalBody;

        // Public properties
        public string PlanetName => horizontalPlanet?.Name ?? "Unnamed Planet";
        // Planet phase angle
        public float PhaseAngle => (float)(horizontalPlanet?.PhaseAngle ?? 0.0);
        // Planet's brightness magnitude
        public float Magnitude => (float)(horizontalBody?.Magnitude);

        //Positions for 3D and screen-space
        public Vector3 Position3D { get; private set; }
        public Vector3 LocalScale { get; private set; }
        public Vector2 Position2D { get; private set; }

        /// <summary>
        /// Constructor for queue initialization of planet (like Star and Sun)
        /// </summary>
        public Planet(HorizontalPlanet hPlanet, float drawnDistance = 74f)
        {
            horizontalPlanet = hPlanet;
            horizontalBody = hPlanet;
            DrawnDistance = drawnDistance;

            // Load prefab from Resources (same naming pattern)
            GameObject planetPrefab = Resources.Load<GameObject>("Prefabs/Planet");

            // compute initial world position
            UpdateTransformFromHorizontal();

            // spawn prefab
            go = Object.Instantiate(planetPrefab, Position3D, Quaternion.identity);
        }

        /// <summary>
        /// Enable or disable the planet’s GameObject.
        /// </summary>
        public void ToggleState()
        {
            if (go != null)
            {
                go.SetActive(!go.activeSelf);
            }
        }

        /// <summary>
        /// Update the planet’s world position based on a new HorizontalPlanet snapshot.
        /// </summary>
        public void UpdatePlanet(HorizontalPlanet hPlanet)
        {
            horizontalPlanet = hPlanet;
            horizontalBody = hPlanet;

            UpdateTransformFromHorizontal();

            if (go != null)
            {
                go.transform.position = Position3D;
                go.transform.localScale = LocalScale;
            }
        }


        /// <summary>
        /// One-time initialization when first instantiated in Unity.
        /// </summary>
        public void FromHorizontal(HorizontalPlanet hPlanet, float drawnDistance = 50f)
        {
            horizontalPlanet = hPlanet;
            horizontalBody = hPlanet;
            DrawnDistance = drawnDistance;

            UpdateTransformFromHorizontal();
        }

        /// <summary>
        /// Updates position when the Engine sends a new horizontal state.
        /// </summary>
        public void ApplyHorizontal(HorizontalPlanet hPlanet)
        {
            horizontalPlanet = hPlanet;
            horizontalBody = hPlanet;

            UpdateTransformFromHorizontal();
        }

        /// <summary>
        /// Computes world and screen-space position.
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

            LocalScale = ComputePlanetScale(Magnitude, PhaseAngle);

            var cam = Camera.main;
            if (cam != null)
            {
                Vector3 sp = cam.WorldToScreenPoint(Position3D);
                Position2D = new Vector2(sp.x, sp.y);
            }
        }

        /// <summary>
        /// Converts Alt/Az to Unity position
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

        /// <summary>
        /// Scales planets based on magnitude and phase angle.
        /// </summary>
        private static Vector3 ComputePlanetScale(float magnitude, float phaseAngle)
        {
            // Base size mapping 
            float size = Mathf.Clamp(1.0f - 0.05f * magnitude, 0.4f, 1.5f);

            // Add subtle phase scaling
            float phaseFactor = Mathf.Clamp01(Mathf.Cos(phaseAngle * Mathf.Deg2Rad));

            size *= Mathf.Lerp(0.7f, 1.0f, phaseFactor);
            return new Vector3(size, size, size);
        }
    }
}