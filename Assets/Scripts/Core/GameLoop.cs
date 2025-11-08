using Assets.Scripts.CelestialBodies;
using Assets.Scripts.UI;
using ChargerAstronomyEngine.CosineKittyAstronomy.Enums;
using ChargerAstronomyShared.Contracts.Models;
using ChargerAstronomyShared.Contracts.Repositories;
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

    public IEngineService engineService;

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


    IEngineService engineService;
    StarQueue starQueue;
    List<List<Star>> starList;
    List<MessierObject> messierList;
    List<Planet> planetList;


    BlockingCollection<TileId> ActivationQueue;
    BlockingCollection<TileId> DeactivationQueue;
    BlockingCollection<TileId> UpdateTransformQueue;


    Sun sun;
    Moon moon;
    InputData inputData = new();

    // Calendar Time
    //CalendarDateTime time = new CalendarDateTime();
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        starList = new();
        messierList = MessierRetrieval.GetMessier();
        LocalObjectRetrieval.GetLocalObjects(ref moon, ref planetList, ref sun);
        StartCoroutine(InitalizeSky());
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSimulation(Time.deltaTime);
        
    }


    public void UpdateSimulation(float deltaTime)
    {
        /// <summary>
        /// Main method to handle all things that need to be done in a frame
        /// </summary>
        /// 


        // temporary way of accessing the inputs
        SetCameraPosition();

        //start engine service
        var task = Task.Run(() => engineService.Step(deltaTime, inputData.camDir, inputData.fov));

 
            TileId tileId;
            while(ActivationQueue.TryTake(out tileId))
            {
                Debug.Log((starList.Count - 1) - tileId.Index );
                if((starList.Count - 1) - tileId.Index >= 0)
                    foreach (Star starToActivate in starList[tileId.Index])
                    {
                    //starToActivate.ToggleState();     // activate TODO: need new method
                    }
            }
    
            while(DeactivationQueue.TryTake(out tileId))
            {
                Debug.Log((starList.Count - 1) - tileId.Index);
                if ((starList.Count - 1) - tileId.Index >= 0)
                    foreach (Star starToDeactivate in starList[tileId.Index])
                    {
                    //starToDeactivate.ToggleState();   // deactivate TODO: need new method
                    }
            }

            while(UpdateTransformQueue.TryTake(out tileId))
            {
                Debug.Log((starList.Count - 1) - tileId.Index);
                if ((starList.Count - 1) - tileId.Index >= 0)
                    foreach (Star starToUpdate in starList[tileId.Index])
                    {
                    // starToUpdate.UpdateStar();          // update star TODO: need new method
                    }
            }





            task.Wait();

    }

    public void SetCameraPosition()
    {
        var inputs = InputContainer.Container;
        inputData.fov = inputs.HorizontalFOV;
        inputData.camDir = new System.Numerics.Vector3(inputs.RotationVector.x, inputs.RotationVector.y, inputs.RotationVector.z);
    }

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
        starQueue = new StarQueue(1000);
        engineService = starQueue.GetEngineService();
        ActivationQueue = engineService.ActivationQueue;
        DeactivationQueue = engineService.DeactivationQueue;
        UpdateTransformQueue = engineService.UpdateTransformQueue;
        engineService.StartServices();
        while (!starQueue.IsCompleted())
        {
            if (starQueue.TryDequeue(ref starList))
            {
            }
            yield return null;
        }
        
        



        
        
    }
}
