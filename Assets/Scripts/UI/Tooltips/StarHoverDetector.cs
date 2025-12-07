using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.CelestialBodies;

namespace Assets.Scripts.UI.Tooltips
{
    public class StarHoverDetector : MonoBehaviour
    {
        [Header("Picking")]
        public Camera cameraOverride;          
        public float maxPickDistance = 5000f;
        [Range(0.05f, 5f)]
        public float radiusMultiplier = 0.6f; 

        private Star currentHovered;

        void Update()
        {
            Camera cam = cameraOverride != null ? cameraOverride : Camera.main;
            if (cam == null)
                return;

            IReadOnlyList<Star> stars = Star.AllStars;
            if (stars == null || stars.Count == 0)
            {
                UpdateHover(null);
                return;
            }

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            Star bestStar = null;
            float bestT = maxPickDistance;

            for (int i = 0; i < stars.Count; i++)
            {
                Star s = stars[i];

                if (!s.IsVisible)        
                    continue;

                float radius = s.LocalScale.x * radiusMultiplier;
                if (radius <= 0f) continue;

                if (RaySphereHit(ray, s.Position3D, radius, out float t) && t < bestT)
                {
                    bestT = t;
                    bestStar = s;
                }
            }

            UpdateHover(bestStar);
        }

        bool RaySphereHit(Ray ray, Vector3 center, float radius, out float t)
        {
            t = 0f;

            Vector3 oc = ray.origin - center;
            float a = Vector3.Dot(ray.direction, ray.direction);
            float b = 2f * Vector3.Dot(ray.direction, oc);
            float c = Vector3.Dot(oc, oc) - radius * radius;

            float disc = b * b - 4f * a * c;
            if (disc < 0f) return false;

            float sqrtDisc = Mathf.Sqrt(disc);
            float t0 = (-b - sqrtDisc) / (2f * a);
            float t1 = (-b + sqrtDisc) / (2f * a);

            if (t0 > 0f)
                t = t0;
            else if (t1 > 0f)
                t = t1;
            else
                return false;

            return true;
        }

        void UpdateHover(Star newHovered)
        {
            if (newHovered == currentHovered)
                return;

            currentHovered = newHovered;

            if (StarTooltipText.Instance == null)
                return;

            if (currentHovered == null)
                StarTooltipText.Instance.Hide();
            else
                StarTooltipText.Instance.ShowAtMouse(currentHovered);
        }
    }
}
