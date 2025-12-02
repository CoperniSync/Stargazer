using UnityEngine;
using ChargerAstronomyShared.Domain.Equatorial;
using ChargerAstronomyShared.Domain.Horizontal;
using ChargerAstronomyShared.Contracts.Models;
using ChargerAstronomyEngine.CosineKittyAstronomy.Enums;
using Assets.Scripts.UI;

namespace Assets.Scripts.CelestialBodies
{
    /// <summary>
    /// Represents a Messier deep-sky object 
    /// in Unity using horizontal coordinates.
    /// Author: Tommy Rodriguez
    /// Created: 2025-10-05
    /// </summary>
    public sealed class MessierObject : CelestialBodyBase, IHorizontal
    {
        private HorizontalMessierObject? horizontalMessier;
        public HorizontalBody HorizontalBody => horizontalBody;

        // ----------------------------
        // Identification
        // ----------------------------
        public string MessierId => horizontalMessier?.MessierId ?? "Unknown M#";
        public string NewGeneralCatalog => horizontalMessier?.NewGeneralCatalog ?? "—";
        public string Type => horizontalMessier?.Type ?? "Unspecified";
        public string Constellation => horizontalMessier?.Constellation ?? "Unknown";
        public string Size => horizontalMessier?.Size ?? "—";
        public string ViewingSeason => horizontalMessier?.ViewingSeason ?? "All Year";
        public string ViewingDifficulty => horizontalMessier?.ViewingDifficulty ?? "Moderate";
        public string CommonName => horizontalMessier?.CommonName ?? "Unnamed Object";

        //  Positions for rendering
        public float Magnitude => (float)(horizontalBody?.Magnitude);

        public Vector3 Position3D { get; private set; }
        public Vector2 Position2D { get; private set; }

        private GameObject go;

        private bool enabled;
        private bool visibilityState; // if the messier object should be rendered

        /// <summary>
        /// Prefab-spawning constructor for Messier objects.
        /// </summary>
        public MessierObject(HorizontalMessierObject hMessier, float drawnDistance = 125f, GameObject prefab = null)
        {
            visibilityState = true;
            enabled = true;



            horizontalMessier = hMessier;
            horizontalBody = hMessier;
            DrawnDistance = drawnDistance;

            UpdateTransformFromHorizontal();

            string type = Type;

            string prefabName = GetPrefabNameForType(type);
            string resourcePath = $"Prefabs/MessierObjects/{prefabName}";

            GameObject messierObjectPrefab = Resources.Load<GameObject>(resourcePath);

            if (messierObjectPrefab == null)
            {
                Debug.LogWarning(
                    $"[MessierObject] Prefab not found for type '{type}' " +
                    $"(expected at '{resourcePath}'). Falling back to DefaultMessier."
                );

                messierObjectPrefab = Resources.Load<GameObject>("Prefabs/MessierObjects/DefaultMessier");
            }

            if (messierObjectPrefab == null)
            {
                Debug.LogError("[MessierObject] No prefab could be loaded for Messier object." +
                               " Check your Resources/Prefabs/MessierObjects setup.");
                return;
            }

            go = Object.Instantiate(messierObjectPrefab, Position3D, Quaternion.identity);
            UpdateVisibility();
            go.name = $"Messier_{MessierId}";
        }
        

        /// <summary>
        /// Updates an existing Messier object's position and scale.
        /// </summary>
        public void UpdateMessier()
        {

            UpdateTransformFromHorizontal();

            if (go != null)
            {
                go.transform.position = Position3D;
            }
        }

        /// <summary>
        /// Enable or disable the Messier object GameObject.
        /// </summary>
        public void ToggleState()
        {
            if (go != null)
            {
                go.SetActive(!go.activeSelf);
            }
        }

        /// <summary>
        /// One-time initialization from horizontal data.
        /// </summary>
        public void FromHorizontal(HorizontalMessierObject hMessier, float drawnDistance = 125f)
        {
            horizontalMessier = hMessier;
            horizontalBody = hMessier;
            DrawnDistance = drawnDistance;

            UpdateTransformFromHorizontal();
        }

        /// <summary>
        /// Applies updated coordinates or metadata from Engine.
        /// </summary>
        public void ApplyHorizontal(HorizontalMessierObject hMessier)
        {
            horizontalMessier = hMessier;
            horizontalBody = hMessier;
            UpdateTransformFromHorizontal();
        }

        /// <summary>
        /// Converts horizontal coordinates to Unity world position and screen projection.
        /// </summary>
        private void UpdateTransformFromHorizontal()
        {
            if (horizontalBody == null)
                return;

            Position3D = ComputeWorldPosition(
                (float)horizontalBody.Altitude,
                (float)horizontalBody.Azimuth,
                DrawnDistance
            );

            // Screen-space position
            var cam = Camera.main;
            if (cam != null)
            {
                var sp = cam.WorldToScreenPoint(Position3D);
                Position2D = new Vector2(sp.x, sp.y);
            }
        }

        /// <summary>
        /// Converts Altitude/Azimuth to Unity world coordinates.
        /// </summary>
        private static Vector3 ComputeWorldPosition(float altitudeDeg, float azimuthDeg, float radius)
        {
            const float Deg2Rad = Mathf.PI / 180f;
            float alt = altitudeDeg * Deg2Rad;
            float az = azimuthDeg * Deg2Rad;

            float cosAlt = Mathf.Cos(alt);
            float sinAlt = Mathf.Sin(alt);
            float cosAz = Mathf.Cos(az);
            float sinAz = Mathf.Sin(az);

            return new Vector3(
                radius * cosAlt * sinAz,    // X = East component
                radius * sinAlt,             // Y = Up (altitude)
                radius * cosAlt * cosAz      // Z = North component
            );
        }

        private void UpdateVisibility()
        {
            if (go != null)
            {
                go.SetActive(enabled);
            }
        }

        /// <summary>
        /// Enable or disable the messier Object's GameObject.
        /// </summary>
        public void SetState(bool state)
        {
            // Not used anymore - Messier objects aren't part of the culling system
        }

        /// <summary>
        /// Function used for toggling messier Objects on and off via UI
        /// </summary>
        public void SetVisible(bool newVisibility)
        {
            visibilityState = newVisibility;
            if (go != null)
            {
                go.SetActive(visibilityState);
            }
        }

        /// <summary>
        /// Update the position of the messier Object's Game Object
        /// </summary>
        public void UpdatePosition()
        {
            UpdateTransformFromHorizontal();

            if (go != null)
            {
                go.transform.position = Position3D;
                // Visibility is controlled solely by visibilityState now
            }
        }
        private static string GetPrefabNameForType(string type)
        {
            if (string.IsNullOrWhiteSpace(type))
                return "DefaultMessier";

            switch (type.Trim())
            {
                case "Supernova Remnant":
                    return "SupernovaRemnant";

                case "Globular Cluster":
                    return "GlobularCluster";

                case "Open Cluster":
                    return "OpenCluster";

                case "Diffuse Nebula":
                    return "DiffuseNebula";

                case "Star Cloud":
                    return "StarCloud";

                case "Spiral Galaxy":
                    return "SpiralGalaxy";

                case "Elliptical Galaxy":
                    return "EllipticalGalaxy";

                case "Lenticular (S0) Galaxy":
                    return "LenticularGalaxy";

                case "Irregular Galaxy":
                    return "IrregularGalaxy";

                case "Planetary Nebula":
                    return "PlanetaryNebula";

                case "Double Star":
                    return "DoubleStar";

                case "Group/Asterism":
                    return "GroupAsterism";

                default:
                    Debug.LogWarning($"[MessierObject] Unrecognized Messier type '{type}', using DefaultMessier prefab.");
                    return "DefaultMessier";
            }
        }

    }
}