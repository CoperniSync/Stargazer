using Assets.Scripts.CelestialBodies;
using ChargerAstronomyShared.Contracts.Models;
using ChargerAstronomyShared.Contracts.Repositories;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

namespace Assets.Scripts.CelestialBodies.Constellations
{

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

        IEngineService<IHorizontal> engine;
        /// <summary>
        /// Constructs a line segment based upon two stars
        /// </summary>
        /// <param name="starA"> The star at the first endpoint of the segemnt</param>
        /// <param name="starB"> The star at the second endpoint of the segemnt</param>
        public ConstellationSegment(Star starA, Star starB, IEngineService<IHorizontal> engineIn)
        {

            endpoint1 = starA;
            endpoint2 = starB;
            engine = engineIn;

            go = new("ConstellationSegment");
            lineRenderer = go.AddComponent<LineRenderer>();

            lineRenderer.sortingLayerName = "Constellations";
            lineRenderer.sortingOrder = 10;

            lineRenderer.SetPosition(0, endpoint1.Position3D);
            lineRenderer.SetPosition(1, endpoint2.Position3D);
            lineRenderer.widthMultiplier = 0.05f;
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
        public ConstellationSegment(Star starA, Star starB, IEngineService<IHorizontal> engineIn, string name)
        {

            endpoint1 = starA;
            endpoint2 = starB;
            go = new(name);
            engine = engineIn;
            lineRenderer = go.AddComponent<LineRenderer>();

            lineRenderer.sortingLayerName = "Constellations";
            lineRenderer.sortingOrder = 10;

            lineRenderer.SetPosition(0, endpoint1.Position3D * 1.2f);
            lineRenderer.SetPosition(1, endpoint2.Position3D * 1.2f);
            lineRenderer.widthMultiplier = 0.3f;
            List<Material> matList = new List<Material>();
            matList.Add(Resources.Load<Material>("Materials/Constellation"));
            lineRenderer.SetMaterials(matList);

        }

        public void addParent(GameObject parent)
        {
            go.transform.parent = parent.transform;
        }



        /// <summary>
        /// Update the position and orientation of the segment 
        /// </summary>
        public void UpdatePosition()
        {
            bool e1Visible = endpoint1.IsVisible;
            bool e2Visible = endpoint2.IsVisible;

            bool lineActive = e1Visible || e2Visible;

            ForceUpdate();

            go.transform.position = GetMidpoint();

            SetState(lineActive);

            lineRenderer.SetPosition(0, endpoint1.Position3D * 1.2f);
            lineRenderer.SetPosition(1, endpoint2.Position3D * 1.2f);

        }
        public void SetState(bool state)
        {
            if (go != null)
            {
                go.SetActive(state);
            }
        }

        public Vector3 GetMidpoint()
        {
            return (endpoint1.Position3D + endpoint2.Position3D) / 2f;
        }

        private void ForceUpdate()
        {
            engine.ForceStarUpdate(endpoint1);
            engine.ForceStarUpdate(endpoint2);
        }


        public bool IsOnScreen()
        {
            return endpoint1.IsVisible || endpoint2.IsVisible;
        }
    }
}