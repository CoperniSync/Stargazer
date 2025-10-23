using Assets.Scripts.CelestialBodies;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GameLoop : MonoBehaviour
{
    /// <summary>
    /// Class that controls the core flow of the program.
    /// Assigns tasks to both the frontend and the backend
    /// </summary>
    
    // Author: Morgan Hendon 10/2025


    StarQueue starQueue;
    List<Star> starList;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        starQueue = new StarQueue(1000);
        starList = new();
        
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

        if(starQueue.TryDequeue(ref starList))
        {
            Debug.Log(starList.Count);
        }

    }

    static public void Print(string str)
    {
        Debug.Log(str);
    }

    static public string GetProjectPath()
    {
        ///<summary>
        /// Should return the path to the folder that this repo is in as a <see cref="string"/>
        /// </summary>
        return new DirectoryInfo(Application.streamingAssetsPath).Parent.Parent.Parent.ToString();
    }

    IEnumerator GetStars()
    {
        /// <summary>
        /// coroutine for getting all the stars
        /// </summary>
        /// 

        return null;
    }
}
