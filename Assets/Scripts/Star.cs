using UnityEngine;
using ChargerAstronomyShared.Domain.Equatorial;
using ChargerAstronomyShared.Domain.Horizontal;
using NUnit.Framework.Internal.Commands;





namespace Stargazers.Unity
{
    /// <summary>
    /// A star that converts its celestial coordinates to a Unity position.
    /// Aurtho: Tommy Rodriguez
    /// Created 9/25/2025
    /// Refactored 9/25/2025
    /// </summary>



    public sealed class Star : CelestialBodyBase
    {
        // Strongly typed identity
        private EquatorialStar starEq;

        // Snapshot as pushed by by backend (Az/Alt/Mag, etc.)
        private HorizontalStar starHz;

        // Set Hipparcs ID if available, otherwise use equatorial star's Id, otherwise 0
        public int HipparcosId => starHz?.HipparcosId ?? starEq?.HipparcosId ?? 0;

        // Use star name from the horizontal data if available, otherwise use the catalogs proper name, otherwise Unamed Star.
        public string StarName => string.IsNullOrWhiteSpace(starHz?.StarName)
                                    ? (string.IsNullOrWhiteSpace(starEq?.ProperName) ? "Unnamed Star" : starEq.ProperName)
                                    : starHz.StarName);
        // Apparent brightness of the star
        public float Magnitude { get { return starHz?.Magnitude; } }

        // World Position
        public Vector3 Position3D { get; private set; }
        
        //Screen-space position helper
        public Vector2 Position2D { get; private set; }

        public void FromHorizontal(HorizontalStar hstar, float drawnDistance = 74f)
        {
            starHz = hstar;
            horizontalBody = hstar;
            equatorialBody = null;
            DrawnDistance = drawnDistance;

        }








    }

}

