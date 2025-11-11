using Assets.Scripts.CelestialBodies;
using Assets.Scripts.UI;
using ChargerAstronomyEngine.CosineKittyAstronomy.Enums;
using ChargerAstronomyShared.Contracts.Models;
using ChargerAstronomyShared.Contracts.Repositories;
using ChargerAstronomyShared.Domain;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Unity.Jobs;
using UnityEngine;


//Jobs
//==============================================================

/// <summary>
/// Job for updating the Engine Service
/// </summary>
struct EngineUpdate : IJob
{
    public System.Numerics.Vector3 cameraDirection;
    public float horizontalFOV;

    public IEngineService<Star> engineService;

    public float deltaTime;
    public void Execute()
    {
        
    }
}


// Structs
//===============================================================

//struct for storing the input data in the form the engine needs it
public struct InputData
{

    //Camera

    // the direction of the camera
    public System.Numerics.Vector3 camDir;

    //FOV of the Camera
    public float fov;


}










// Game Loop Class
//===============================================================

public class GameLoop : MonoBehaviour
{
    /// <summary>
    /// Class that controls the core flow of the program.
    /// Assigns tasks to both the frontend and the backend
    /// </summary>

    // Author: Morgan Hendon 10/2025

    // job related
    JobHandle engineHandle;

    IEquatorialCalculator equatorialCalculator;
    IEngineService<Star> engineService;
    StarQueue starQueue;
    List<List<Star>> starList;
    List<MessierObject> messierList;
    List<Planet> planetList;


    BlockingCollection<Star> ActivationQueue;
    BlockingCollection<Star> DeactivationQueue;
    BlockingCollection<Star> UpdateTransformQueue;


    Sun sun;
    Moon moon;
    InputData inputData = new();

    // Calendar Time
    //CalendarDateTime time = new CalendarDateTime();
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        starList = new();
        //messierList = MessierRetrieval.GetMessier(); 
        LocalObjectRetrieval.GetLocalObjects(ref moon, ref planetList, ref sun);
        starQueue = new StarQueue(1000 , "AllStars.csv");

        // set up engine service
        engineService = starQueue.GetEngineService();
        ActivationQueue = engineService.ActivationQueue;
        DeactivationQueue = engineService.DeactivationQueue;
        UpdateTransformQueue = engineService.UpdateTransformQueue;
        equatorialCalculator = engineService.StartServices();

        //start pulling items from starQueue
        StartCoroutine(InitalizeSky());
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSimulation(Time.deltaTime);
        
    }

    /// <summary>
    /// Main method to handle all things that need to be done in a frame
    /// </summary>
    /// 
    private void UpdateSimulation(float deltaTime)
    {
       

        // temporary way of accessing the inputs
        SetCameraPosition();

        //start engine service
        var task = Task.Run(() => engineService.Step(deltaTime, inputData.camDir, inputData.fov));

 
            Star pulledStar;
            while(ActivationQueue.TryTake(out pulledStar))
            {
                //Debug.Log("Item From Activation Queue: " + pulledStar.HipparcosId);
                pulledStar.SetState(true);     // activate TODO: need new method
            }
    
            while(DeactivationQueue.TryTake(out pulledStar))
            {
                //Debug.Log("Item From Deactivation Queue: " +pulledStar.HipparcosId);
                pulledStar.SetState(false);   // deactivate TODO: need new method
            }

            while(UpdateTransformQueue.TryTake(out pulledStar))
            {
                //Debug.Log("Item From Update Queue" + pulledStar.HipparcosId);
            pulledStar.SetState(true);
            pulledStar.UpdateStar();
            }


        moon.UpdateMoon();
        sun.UpdateSun();

        foreach (Planet planet in planetList)
        {
            planet.UpdatePlanet();
        }



        task.Wait();

    }


    /// <summary>
    /// Updates the engine's camera position based on the Input Container
    /// </summary>
    public void SetCameraPosition()
    {
        var inputs = InputContainer.Container;
        inputData.fov = inputs.HorizontalFOV;
        inputData.camDir = new System.Numerics.Vector3(inputs.RotationVector.x, inputs.RotationVector.y, inputs.RotationVector.z);
    }


    /// <summary>
    ///  Set 
    /// </summary>
    public void SetLocationAndTime()
    {
        var inputs = InputContainer.Container;
        var newTime = new CalendarDateTime(inputs.Year, inputs.Time.Month, inputs.Time.Day, inputs.Time.Hour, inputs.Time.Minute, inputs.Time.Second);
        var newObserver = new Observer(Convert.ToDouble(inputs.LatitudeDeg) + (Convert.ToDouble(inputs.LatitudeMin)/60.0), Convert.ToDouble(inputs.LongitudeMin) + (Convert.ToDouble(inputs.LongitudeDeg) / 60.0), 0.0);
        equatorialCalculator.UpdateTimeAndLocation(newTime,newObserver);
    }


    /// <summary>
    /// method for accessing the folder that contains all the github repos
    /// </summary>
    /// <returns></returns>
    static public string GetProjectPath()
    {
        ///<summary>
        /// Should return the path to the folder that this repo is in as a <see cref="string"/>
        /// </summary>
        return new DirectoryInfo(Application.streamingAssetsPath).Parent.Parent.Parent.ToString();
    }

    IEnumerator InitalizeSky()
    {
        /// <summary>
        /// coroutine for getting all the objects for the sky
        /// </summary>
        /// 

        // get the stars and put them in star list
        
        while (!starQueue.IsCompleted())
        {
            if (starQueue.TryDequeue(ref starList))
            {
            }
            yield return null;
        }
        
        



        
        
    }
}
