using UnityEngine;
using System.Threading.Tasks;
using ChargerAstronomyShared;
using System;

///<summary>
/// Base class for all celestial objects in the game.
/// Backend-driven updates to positions
/// </summary>

namespace Stargazer.Unity
{


    [Serializable]
    public struct Equatorial // J2000 date
    {
        public double rightAscension; // in hours
        public double declination; // in degrees
    }

    [Serializable]
    public struct Horizontal
    {
        public double azimuth; // in degrees
        public double altitude; // in degrees
    }


}
