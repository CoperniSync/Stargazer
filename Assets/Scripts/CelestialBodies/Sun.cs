using UnityEngine;
using ChargerAstronomyShared.Domain.Equatorial;
using ChargerAstronomyShared.Domain.Horizontal;

namespace Stargazers.Unity
{
    /// <summary>
    /// Represents the Sun as a Unity object using horizontal coordinates.
    /// Author: Tommy Rodriguez
    /// Created: 2025-10-05
    /// </summary>
    public sealed class Sun : CelestialBodyBase
    {
        private HorizontalSun?              horizontalSun;
        private HorizontalBody?             horizontalBody;
        private EquatorialCelestialBody?    equatorialBodyTyped;

        // The Sun's name
        public string SunName   => "Sun";

        // The Sun's brightness magnitude
        public float Magnitude  => (float)(horizontalBody?.Magnitude ?? equatorialBodyTyped?.Magnitude);
 

        // Positions for rendering
        public Vector3 Position3D { get; private set; }
        public Vector2 Position2D { get; private set; }

        /// <summary>
        /// One-time initialization from horizontal data.
        /// </summary>
        public void FromHorizontal(HorizontalSun hSun, float drawnDistance = 100f)
        {
            horizontalSun       = hSun;
            horizontalBody      = hSun;
            equatorialBodyTyped = hSun.EquatorialBody;
            equatorialBody      = hSun.EquatorialBody;
            DrawnDistance       = drawnDistance;

            UpdateTransformFromHorizontal();
        }

        /// <summary>
        /// Applies updated horizontal coordinates from the Engine.
        /// </summary>
        public void ApplyHorizontal(HorizontalSun hSun)
        {
            horizontalSun       = hSun;
            horizontalBody      = hSun;
            equatorialBodyTyped = hSun.EquatorialBody;
            equatorialBody      = hSun.EquatorialBody;

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

            transform.position      = Position3D;
            transform.localScale    = ComputeSunScale(Magnitude);

            // Screen-space for UI
            var cam = Camera.main;
            if (cam != null)
            {
                var sp      = cam.WorldToScreenPoint(Position3D);
                Position2D  = new Vector2(sp.x, sp.y);
            }
        }

        /// <summary>
        /// Converts Altitude/Azimuth to a Unity world position at a given radius.
        /// </summary>
        private static Vector3 ComputeWorldPosition(float altitudeDeg, float azimuthDeg, float radius)
        {
            const float Deg2Rad = Mathf.PI / 180f;
            float alt           = altitudeDeg * Deg2Rad;
            float az            = azimuthDeg * Deg2Rad;

            float cosAlt        = Mathf.Cos(alt);
            float sinAlt        = Mathf.Sin(alt);
            float cosAz         = Mathf.Cos(az);
            float sinAz         = Mathf.Sin(az);

            return new Vector3(
                radius * cosAz * cosAlt,
                radius * sinAlt,
                radius * cosAlt * sinAz
            );
        }

        /// <summary>
        /// Computes a fixed but slightly dynamic Sun scale.
        /// -- Can be dropped if we dont want a dynamic sun -- Tommy
        /// </summary>
        private static Vector3 ComputeSunScale(float magnitude)
        {

            float baseSize          = 5.0f;
            float brightnessAdjust  = Mathf.Clamp(1.0f - (magnitude / -26.74f), 0.85f, 1.15f);
            float finalSize         = baseSize * brightnessAdjust;

            return new Vector3(finalSize, finalSize, finalSize);
        }
    }
}
