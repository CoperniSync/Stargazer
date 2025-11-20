using Assets.Scripts.CelestialBodies;
using UnityEngine;
using System.Collections.Generic;

// Author: Morgan Hendon
// Created On: 11/15/2025

/// <summary>
/// Class that represents a line segment made between two Stars
/// </summary>
public class ConstellationSegment
{
    public Star endpoint1;
    public Star endpoint2;

    LineRenderer lineRenderer;
    GameObject go;
    /// <summary>
    /// Constructs a line segment based upon two stars
    /// </summary>
    /// <param name="starA"> The star at the first endpoint of the segemnt</param>
    /// <param name="starB"> The star at the second endpoint of the segemnt</param>
    public ConstellationSegment(Star starA, Star starB)
    {

        endpoint1 = starA;
        endpoint2 = starB;

        go = new("ConstellationSegment");
        lineRenderer = go.AddComponent<LineRenderer>();

        lineRenderer.SetPosition(0, endpoint1.Position3D);
        lineRenderer.SetPosition(1, endpoint2.Position3D);
        lineRenderer.widthMultiplier = 0.2f;
        List<Material> matList = new List<Material>();
        matList.Add(Resources.Load<Material>("Materials/Constellation"));
        lineRenderer.SetMaterials(matList);

    }
    /// <summary>
    /// Constructs a line segment based upon two stars
    /// </summary>
    /// <param name="starA"> The star at the first endpoint of the segemnt</param>
    /// <param name="starB"> The star at the second endpoint of the segemnt</param>
    /// <param name="name"> The name of the GameObject that is created</param>
    public ConstellationSegment(Star starA, Star starB, string name)
    {

        endpoint1 = starA;
        endpoint2 = starB;
        go = new(name);
        lineRenderer = go.AddComponent<LineRenderer>();
        
        lineRenderer.SetPosition(0, endpoint1.Position3D);
        lineRenderer.SetPosition(1, endpoint2.Position3D);
        lineRenderer.widthMultiplier = 0.2f;
        List<Material> matList = new List<Material>();
        matList.Add(Resources.Load<Material>("Materials/Constellation"));
        lineRenderer.SetMaterials(matList);

    }



    /// <summary>
    /// Update the position and orientation of the segment 
    /// </summary>
    public void UpdatePosition()
    {
        bool e1Visible = endpoint1.IsVisible;
        bool e2Visible = endpoint2.IsVisible;

        bool lineActive = e1Visible && e2Visible;

        SetState(lineActive);

        lineRenderer.SetPosition(0, endpoint1.Position3D);
        lineRenderer.SetPosition(1, endpoint2.Position3D);

    }
    public void SetState(bool state)
    {
        if (go != null)
        {
            go.SetActive(state);
        }
    }

}
