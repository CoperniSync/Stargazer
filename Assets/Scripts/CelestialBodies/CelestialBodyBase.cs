using UnityEngine;
using ChargerAstronomyShared.Domain.Equatorial;
using ChargerAstronomyShared.Domain.Horizontal;

/// <summary>
/// Base class for all celestial bodies in the game.
/// Author: Tommy Rodriguez
/// Created 9/21/2025
/// Refactored 9/25/2025
/// </summary>

namespace Assets.Scripts.CelestialBodies
{
    public abstract class CelestialBodyBase
    {
        protected EquatorialCelestialBody equatorialBody;
        protected HorizontalBody horizontalBody;

        // Horizontal properties
        /// <summary>
        /// Rotation from North (X+), in degrees
        /// </summary>
        public float Azimuth => horizontalBody != null ? (float)horizontalBody.Azimuth : 0f; //Helps handles against null reference
        /// <summary>
        /// Rotation from Y=0, in degrees.
        /// </summary>
        public float Altitude => horizontalBody != null ? (float)horizontalBody.Altitude : 0f;
        /// <summary>
        /// Distance from (0, 0, 0)
        /// </summary>
        public float Distance => horizontalBody != null ? (float)horizontalBody.Distance : 0f;

        // Equatorial properties
        public double RightAscension => equatorialBody != null ? equatorialBody.RightAscension : 0d;
        public double Declination => equatorialBody != null ? equatorialBody.Declination : 0d;
        public double Magnitude => equatorialBody != null ? equatorialBody.Magnitude : 0d;
        public double DistanceLy => equatorialBody != null ? equatorialBody.Distance : 0d;


        public float DrawnDistance { get; set; }

        protected CelestialBodyBase() { }

        protected CelestialBodyBase(HorizontalBody horizontal, float drawnDitance = 74f)
        {
            horizontalBody = horizontal;
            DrawnDistance = DrawnDistance;
        }

        public void SetHorizontal(HorizontalBody horizontal, float? drawnDistance = null)
        {
            horizontalBody = horizontal;
            if (drawnDistance.HasValue) DrawnDistance = drawnDistance.Value;
        }


        /// <summary>
        /// Converts the Azimuth/Altitude and DrawnDistance into a Unity Vector3 position
        /// </summary>
        protected Vector3 GetLocation()
        {
            // Use the same fallback scaling as the original
            float dist = DrawnDistance > 70f
                ? DrawnDistance
                : 50f + (DrawnDistance / 31f) * (70f - 50f);

            float altRad = Altitude * Mathf.Deg2Rad;
            float azRad = Azimuth * Mathf.Deg2Rad;

            return new Vector3(
                dist * (Mathf.Cos(azRad) * Mathf.Cos(altRad)),
                dist * Mathf.Sin(altRad),
                dist * Mathf.Cos(altRad) * Mathf.Sin(azRad)
            );
        }


    }
}