using Assets.Scripts.CelestialBodies;
using ChargerAstronomyShared.Contracts.Models;
using System.Collections;
using UnityEngine;

public class UnityInterface : MonoBehaviour
{
    //class that holds the core game loop

    PageRequest starPageRequest; //used for requesting Stars
    PageResult<Star?> starPageResult;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        starPageRequest = new();
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
