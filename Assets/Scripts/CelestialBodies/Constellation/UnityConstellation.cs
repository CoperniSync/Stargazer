using Assets.Scripts.CelestialBodies;
using Assets.Scripts.UI;
using ChargerAstronomyEngine.Streaming;
using ChargerAstronomyShared.Contracts.Models;
using ChargerAstronomyShared.Contracts.Repositories;
using ChargerAstronomyShared.Domain.Equatorial;
using System;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnityConstellation
{ 
    
    private bool enabled = false;
    private bool labelVisible = false;
    private GameObject go = null;
    private GameObject label = null;
    private List<ConstellationSegment> segments = new List<ConstellationSegment>();
    private IEngineService<IHorizontal> engine;
    /// <summary>
    /// Creates a constellation in the sky between already defined stars.
    /// </summary>
    /// <param name="starList">The list of stars that the star data will be pulled from</param>
    /// <param name="constellation">Information that the constellation will be created from</param>
    /// <param name="initalState"> Whether the constellation should start visible</param>
    /// <remarks>SetState(lineActive);
    /// Ensure starList has been populated prior to calling this constructor.
    /// </remarks>
    public UnityConstellation(Constellation constellation, List<Star> starList, IEngineService<IHorizontal> engineService, bool initalState = true)
    {
        // way of finding stars should be optimized at some point
        go = new GameObject(constellation.ConstellationName);
        SetVisible(initalState);
        engine = engineService;
      

        foreach (Tuple<int, int> endpoints in constellation.ConstellationLines)
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
                var newSegment = new ConstellationSegment(endpoint1, endpoint2, engine, constellation.ConstellationName);
                newSegment.addParent(go);
                segments.Add(newSegment);

                
            }
        }//end outer foreach
        go.transform.position = CalcuateMiddle();

        GameObject labelPrefab = Resources.Load<GameObject>("Prefabs/Text");
        label = UnityEngine.Object.Instantiate(labelPrefab, Vector3.zero,Quaternion.identity);
        label.transform.parent = go.transform;
        TextMeshPro textComp = label.GetComponent<TextMeshPro>();
        textComp.text = constellation.ConstellationName;
    }


    /// <summary>
    /// Toggles the visibility of the Constellation
    /// </summary>
    /// <param name="state"></param>
    public void SetVisible(bool state)
    {
        enabled = state;
        if (go != null)
        {
            go.SetActive(state);
        }
    }

    public void SetLabelVisible(bool state)
    {
        labelVisible = state;
        if (label != null)
        {
            label.SetActive(state);
        }
    }


    public void UpdatePosition()
    {
        if(!IsOffScreen())
        {
            if(enabled)
            {
                go.SetActive(true);

            }
        }
        else
        {
            go.SetActive(false);
        }

        go.transform.position = CalcuateMiddle();
        foreach (ConstellationSegment segment in segments)
        {
            segment.UpdatePosition();
        }

   
        // update the position and orientation of the label
        //
        var inputs = InputContainer.Container;
        var cameraDirection = new Vector3(-inputs.RotationVector.y, 0f, inputs.RotationVector.x);

        // position
        label.transform.localPosition = Vector3.zero;

        //rotation
        label.transform.LookAt(new Vector3(0f, 0.0f, 00f) - 64 * cameraDirection);

        

        label.transform.forward = -label.transform.forward;

        //scale
        float scaleRatio = (inputs.HorizontalFOV * Mathf.Rad2Deg) / 60f;
        label.transform.localScale = new(scaleRatio,scaleRatio,scaleRatio);
    }



    private Vector3 CalcuateMiddle()
    {
        var segCount = 0f;
        var middle = new Vector3(0f, 0f, 0f);
        foreach (ConstellationSegment segment in segments)
        {
            middle = middle + segment.GetMidpoint();
            segCount++;
        }
        return middle / segCount;
    }

    /// <summary>
    /// returns true if the ENITRE constellation is off screen
    /// </summary>
    /// 
    private bool IsOffScreen()
    {
        bool isOffScreen = true;
        foreach (ConstellationSegment segment in segments)
        {
            isOffScreen = !segment.IsOnScreen() && isOffScreen;
            if(!isOffScreen)
            {
                break;
            }
        }
        return isOffScreen;
    }


    /// <summary>
    /// returns true if the ENITRE constellation is on screen
    /// </summary>
    /// 
    private bool IsOnScreen()
    {
        bool isOnScreen = true;
        foreach (ConstellationSegment segment in segments)
        {
            isOnScreen = segment.IsOnScreen() && isOnScreen;
            if (!isOnScreen)
            {
                break;
            }
        }
        return isOnScreen;
    }


    /// <summary>
    /// returns if the constellation a given percentage of the constellation is on screen
    /// </summary>
    /// 
    private bool isOnScreen(float threshold)
    {
        int onScreen = 0;
        int total = 0;
        foreach (ConstellationSegment segment in segments)
        {
            if(segment.IsOnScreen())
            {
                onScreen++;
            }
            total++;
        }
        return onScreen/total >= threshold;
    }

}

