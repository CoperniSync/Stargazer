using ChargerAstronomyEngine.Data.LocalObjects;
using ChargerAstronomyShared.Contracts.Models;
using ChargerAstronomyShared.Domain.Horizontal;
using Assets.Scripts.CelestialBodies;
using NUnit.Framework;
using System.Collections.Generic;

// Author: Morgan Hendon FA 2025
public class LocalObjectRetrieval
{
    public static void GetLocalObjects(ref Moon moon,ref List<Planet> planets,ref Sun sun)
    {
        HorizontalMoon hMoon = MoonSingleton.Instance.CreateMoon();
        moon = new Moon(hMoon, 95f);
        
        
        planets = new List<Planet>();
        foreach (HorizontalPlanet hPlanet in PlanetsSingleton.Instance.CreatePlanets())
        {
            Planet newPlanet = new Planet(hPlanet, 50f);
            planets.Add(newPlanet);
        }
        

        HorizontalSun hSun = SunSingleton.Instance.CreateSun();
        sun = new Sun(hSun, 100f);

    }

}
