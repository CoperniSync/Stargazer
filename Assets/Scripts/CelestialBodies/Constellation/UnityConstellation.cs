using Assets.Scripts.CelestialBodies;
using ChargerAstronomyShared.Domain.Equatorial;
using System;
using System.Collections.Generic;
using UnityEngine;

public class UnityConstellation
{

    bool enabled = false;
    GameObject go = null;
    List<ConstellationSegment> segments = new List<ConstellationSegment>();


    /// <summary>
    /// Creates a constellation in the sky between already defined stars.
    /// </summary>
    /// <param name="starList">The list of stars that the star data will be pulled from</param>
    /// <param name="constellation">Information that the constellation will be created from</param>
    /// <remarks>
    /// Ensure starList has been populated prior to calling this constructor.
    /// </remarks>
    public UnityConstellation(Constellation constellation, List<Star> starList)
    {
        // way of finding stars should be optimized at some point
        go = new GameObject(constellation.ConstellationName);
        foreach(Tuple<int, int> endpoints in constellation.ConstellationLines)
        {
            Star endpoint1 = null, endpoint2 = null;
            foreach(Star star in starList)
            {
                if (endpoints.Item1 == star.HipparcosId)
                {
                    endpoint1 = star;
                    if(endpoint2 != null) 
                    {
                        break;
                    }
                }
                if (endpoints.Item2 == star.HipparcosId)
                {
                    endpoint2 = star;
                    if (endpoint1 != null)
                    {
                        break;
                    }
                }

            }//end inner foreach
            if (endpoint1 != null && endpoint2 != null)
            {
                segments.Add(new ConstellationSegment(endpoint1, endpoint2, constellation.ConstellationName));
            }
        }//end outer foreach
    }


    /// <summary>
    /// Toggles the visibility of the Constellation
    /// </summary>
    /// <param name="state"></param>
    public void SetState(bool state)
    {
        enabled = state;
        if (go != null)
        {
            go.SetActive(state);
        }
    }

    public void UpdatePosition()
    {
        foreach(ConstellationSegment segment in segments)
        {
            segment.UpdatePosition();  
        }
    }
}
