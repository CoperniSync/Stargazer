using Assets.Scripts.CelestialBodies;
using ChargerAstronomyShared.Contracts.Models;
using System.Collections;
using UnityEngine;
using ChargerAstronomyShared.Domain.Equatorial;
using ChargerAstronomyEngine.Streaming;
using ChargerAstronomyEngine.Data;
public class UnityInterface : MonoBehaviour
{
    //class that holds the core game loop


    //star instantiation
    PageRequest starPageRequest; //used for requesting Stars
    BoundedInitializationQueue<PageResult<EquatorialStar>> initQueue;
    PageResult<EquatorialStar> starPageResult;
    CsvStarRepository starRepo;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        starPageRequest = new();
        initQueue = new(5);

        
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
