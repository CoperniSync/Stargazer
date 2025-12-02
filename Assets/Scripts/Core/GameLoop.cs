using Assets.Scripts.CelestialBodies;
using Assets.Scripts.Core;
using Assets.Scripts.UI;
using ChargerAstronomyEngine.CosineKittyAstronomy.Enums;
using ChargerAstronomyEngine.Streaming;
using ChargerAstronomyShared.Contracts.Models;
using ChargerAstronomyShared.Contracts.Repositories;
using ChargerAstronomyShared.Domain;
using ChargerAstronomyShared.Domain.Equatorial;
using ChargerAstronomyShared.Domain.Horizontal;
using ChargerAstronomyShared.Domain.Index;
using ChargerAstronomyShared.Domain.SpatialIndex;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

public struct InputData
{
    // Camera direction in HORIZONTAL space (Alt/Az relative to observer)
    public System.Numerics.Vector3 camDirHorizontal;
    public float fov;

    // state of messier Objects
    public bool messierOn;

    // state of constellations
    public bool constellationOn;

    public bool labelsOn;

    public Observer observer;

    public DateTime time;

    public int year;


}

/// <summary>
/// Main game loop controller. Updated to properly handle coordinate transformations
/// and efficient star culling via the heat map system.
/// </summary>
public class GameLoop : MonoBehaviour
{
    private const int SUBDIVISIONS = 3;
    public float speedMult = 1;

    private IEquatorialCalculator equatorialCalculator;
    private IEngineService<IHorizontal> engineService;
    private StarQueue starQueue;

    private List<Star> starList;
    private List<MessierObject> messierList;
    private List<Planet> planetList;
    private List<UnityConstellation> constellationList;

    private BlockingCollection<IHorizontal> ActivationQueue;
    private BlockingCollection<IHorizontal> DeactivationQueue;
    private BlockingCollection<IHorizontal> UpdateTransformQueue;

    private Sun sun;
    private Moon moon;
    private InputData inputData = new();

    // For debugging
    private float debugTimer = 0f;
    private const float DEBUG_INTERVAL = 1f;

    bool starsLoaded = false;

    void Start()
    {
        starList = new List<Star>();
        constellationList = new List<UnityConstellation>();
        planetList = new List<Planet>();
        inputData.messierOn = true;
        inputData.constellationOn = true;
        inputData.observer = new Observer(0.0, 0.0, 0.0);
        var inputs = InputContainer.Container;
        inputData.time = inputs.Time;
        inputData.year = inputs.Year;
        // set up engine service

        // Set up engine service with proper tile index
        ITileIndex tileIndex = new IcosphereTileIndex(SUBDIVISIONS);
        engineService = new EngineService<IHorizontal>(tileIndex);

        // Get references to queues
        ActivationQueue = engineService.ActivationQueue;
        DeactivationQueue = engineService.DeactivationQueue;
        UpdateTransformQueue = engineService.UpdateTransformQueue;

        // Get the calculator
        equatorialCalculator = engineService.StartServices();
        SetLocationAndTime();

        // Initialize star loading
        starQueue = new StarQueue(engineService, 500, "AllStars.csv");

        // Get local objects (planets, sun, moon)
        LocalObjectRetrieval.GetLocalObjects(ref moon, ref planetList, ref sun);

        messierList = MessierRetrieval.GetMessier(engineService);

        // Start initialization coroutine
        StartCoroutine(InitializeSky());
    }

    void Update()
    {
        UpdateSimulation(Time.deltaTime * speedMult);

        // Debug output
        debugTimer += Time.deltaTime;
        if (debugTimer >= DEBUG_INTERVAL)
        {
            debugTimer = 0f;
            if (engineService is EngineService<IHorizontal> engine)
            {
                var stats = engine.GetStats();

                //Debug.Log($"Engine Stats: {stats}");
            }
        }
    }

    private float CalculateMagnitudeCutoff(float fovDegrees)
    {
        float baseLimit = 6.0f;
        float zoomFactor = 60f / Mathf.Max(fovDegrees, 0.1f);
        float magnitudeBoost = Mathf.Log(zoomFactor, 2f) * 1.5f; // ~1.5 mag per doubling of zoom

        return baseLimit + magnitudeBoost;
    }

    private void UpdateSimulation(float deltaTime)
    {

        if (!starsLoaded) return;


        inputData.time = inputData.time.AddSeconds(deltaTime);
        if (inputData.time.Year != 2000)
        {
            inputData.year = inputData.year + (inputData.time.Year - 2000);
            inputData.time = inputData.time.AddYears(2000 - inputData.time.Year);
        }
        SetCameraPosition();

        float magnitudeCutoff = CalculateMagnitudeCutoff(Camera.main.fieldOfView);

        engineService.Step(
            deltaTime,
            inputData.camDirHorizontal.X,
            inputData.camDirHorizontal.Y,
            inputData.camDirHorizontal.Z,
            inputData.fov,
            magnitudeCutoff,
            speedMult
        );


        // important to do this part first otherwise there is a very weird visual bug for some reason

        IHorizontal pulledObject;

        int updateCount = 0;
        while (UpdateTransformQueue.TryTake(out pulledObject, 0))
        {
            pulledObject.UpdatePosition();
            updateCount++;
        }

        // process activation queue
        int activationCount = 0;
        while (ActivationQueue.TryTake(out pulledObject, 0))
        {
            pulledObject.SetState(true);
            activationCount++;
        }
        if (activationCount > 0)
        {
            //Debug.Log(${activationCount} stars");
        }

        // Process deactivation queue
        int deactivationCount = 0;
        while (DeactivationQueue.TryTake(out pulledObject, 0))
        {
            pulledObject.SetState(false);
            deactivationCount++;
        }
        if (deactivationCount > 0)
        {
            //Debug.Log($"{deactivationCount} stars");
        }


        // Update the sun moon and planets 
        equatorialCalculator.UpdatePositionOf(moon.HorizontalMoon);
        moon.UpdatePosition();

        equatorialCalculator.UpdatePositionOf(sun.HorizontalSun);
        sun.UpdatePosition();

        foreach (Planet planet in planetList)
        {
            equatorialCalculator.UpdatePositionOf(planet.HorizontalPlanet);
            planet.UpdatePosition();
        }

        // update constellations
        foreach (var constellation in constellationList)
        {
            constellation.UpdatePosition();
        }
    }

    /// <summary>
    /// set if the messier Objects are being forced from displaying
    /// </summary>
    public void SetMessierVisibility(bool visible)
    {
        inputData.messierOn = visible;
        foreach (MessierObject m in messierList)
        {
            m.SetVisible(visible);
        }
    }


    /// <summary>
    /// sets the visibilty of constelations
    /// </summary>
    public void SetConstellationVisibility(bool visible)
    {
        inputData.constellationOn = visible;
        foreach (UnityConstellation c in constellationList)
        {
            c.SetVisible(visible);
        }
    }

    /// <summary>
    /// sets the visibilty of conste;lations labels
    /// </summary>
    public void SetLabelVisibility(bool visible)
    {
        inputData.constellationOn = visible;
        foreach (UnityConstellation c in constellationList)
        {
            c.SetLabelVisible(visible);
        }
    }

    public void SetSpeedMultiplier(float speed)
    {
        speedMult = speed;
    }

    /// <summary>
    /// Updates the engine's camera position based on the Input Container
    /// </summary>
    public void SetCameraPosition()
    {
        var inputs = InputContainer.Container;
        inputData.fov = inputs.HorizontalFOV;


        inputData.camDirHorizontal = new System.Numerics.Vector3(
            inputs.RotationVector.x,
            inputs.RotationVector.y,
            inputs.RotationVector.z
        );

        inputData.camDirHorizontal = System.Numerics.Vector3.Normalize(inputData.camDirHorizontal);
    }

    public void SetLocationAndTime()
    {
        var inputs = InputContainer.Container;
        inputData.time = inputs.Time;
        inputData.year = inputs.Year;
        var time = new CalendarDateTime(
            inputs.Year,
            inputs.Time.Month,
            inputs.Time.Day,
            inputs.Time.Hour,
            inputs.Time.Minute,
            inputs.Time.Second
        );

        inputData.observer = new Observer(
            Convert.ToDouble(inputs.LatitudeDeg) + (Convert.ToDouble(inputs.LatitudeMin) / 60.0),
            Convert.ToDouble(inputs.LongitudeDeg) + (Convert.ToDouble(inputs.LongitudeMin) / 60.0),
            0.0
        );
        Debug.Log(inputs.Time.ToString());
        Debug.Log(time.ToString());
        equatorialCalculator.UpdateTimeAndLocation(time, inputData.observer);
    }

    public static string GetProjectPath()
    {
        return new DirectoryInfo(Application.streamingAssetsPath).Parent.Parent.Parent.ToString();
    }

    public void GetEngineState(out Vector3 camDirection, out int engineYear, out DateTime engineTime, out Observer engineObserver)
    {
        camDirection = InputContainer.Container.RotationVector;
        engineYear = inputData.year;
        engineTime = inputData.time;
        engineObserver = inputData.observer;
    }

    IEnumerator InitializeSky()
    {
        // Load all stars
        while (!starQueue.IsCompleted())
        {
            starQueue.TryDequeue(ref starList);
            yield return null;
        }

        // get constellation
        ConstellationRetrieval.GetConstellations(ref constellationList, starList, engineService, inputData.constellationOn);


        engineService.SpatialStarIndex.SortAllTilesByMagnitude(); // sorting by magnitude here, there is def a better way but I can think of it rn

        engineService.PlaceStars();
        starsLoaded = true;

        // After stars are loaded, in GameLoop r wherever
        foreach (var tileId in engineService.SpatialStarIndex.TileIndex.Enumerate())
        {
            var starsInTile = engineService.SpatialStarIndex.GetStarsInTile(tileId);
            if (starsInTile.Count >= 0)
            {
                Debug.Log($"Tile {tileId.Index} has {starsInTile.Count} stars");
            }
        }

        Debug.Log($"Finished loading {starList.Count} stars");

        // Load constellations
        // ConstellationRetrieval.GetConstellations(ref constellationList, starList);
    }
}