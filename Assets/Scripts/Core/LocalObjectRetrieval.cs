using ChargerAstronomyEngine.Data.LocalObjects;
using ChargerAstronomyShared.Contracts.Models;
using ChargerAstronomyShared.Domain.Horizontal;
using Assets.Scripts.CelestialBodies;
using NUnit.Framework;
using System.Collections.Generic;

public class LocalObjectRetrieval
{
    public static void GetLocalObjects(ref Moon moon,ref List<Planet> planets,ref Sun sun)
    {
        HorizontalMoon hMoon = MoonSingleton.Instance.CreateMoon();
        moon = new Moon();
        moon.ApplyHorizontal(hMoon);

        planets = new List<Planet>();
        foreach (HorizontalPlanet hPlanet in PlanetsSingleton.Instance.CreatePlanets())
        {
            Planet newPlanet = new();
            newPlanet.ApplyHorizontal(hPlanet);
            planets.Add(newPlanet);
        }
        HorizontalSun hSun = SunSingleton.Instance.CreateSun();
        sun = new Sun();
        sun.ApplyHorizontal(hSun);

    }

}
