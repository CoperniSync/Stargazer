using UnityEngine;
using Assets.Scripts.CelestialBodies;
using System.Collections.Generic;

namespace Assets.Scripts.Rendering
{
    public class StarGpuRenderer : MonoBehaviour
    {
        [Header("Rendering")]
        public Material starMaterial;
        public Mesh starMesh;

        [Header("Settings")]
        public bool billboardToCamera = true;
        [Range(0.5f, 5.0f)]
        public float brightnessMultiplier = 0.87f;
        [Range(0.5f, 10.0f)]
        public float sizeMultiplier = 1.29f;
        [Range(0.0f, 1.0f)]
        public float sizeVariation = 0.7f;
        [Range(1.0f, 100.0f)]
        public float minDistanceClamp = 10.0f;

        private ComputeBuffer positionBuffer;
        private ComputeBuffer scaleBuffer;
        private ComputeBuffer colorBuffer;
        private ComputeBuffer visibilityBuffer;
        private ComputeBuffer argsBuffer;

        private uint[] args = new uint[5] { 0, 0, 0, 0, 0 };
        private Bounds bounds;

        private int maxStars = 100_000;
        private int currentStarCount = 0;

        void Start()
        {
            if (starMesh == null)
            {
                starMesh = CreateQuadMesh();
            }

            bounds = new Bounds(Vector3.zero, Vector3.one * 10000f);

            InitializeBuffers(maxStars);
        }

        void LateUpdate()
        {
            var stars = Star.AllStars;
            if (stars.Count == 0) return;

            if (stars.Count > maxStars)
            {
                // reseet buffers 
                ReleaseBuffers();
                maxStars = Mathf.NextPowerOfTwo(stars.Count);
                InitializeBuffers(maxStars);
            }

            currentStarCount = stars.Count;
            UpdateStarData(stars);

            args[1] = (uint)currentStarCount;
            argsBuffer.SetData(args);

            float currentFOV = Camera.main != null ? Camera.main.fieldOfView : 60f;
            float zoomFactor = 60f / Mathf.Max(currentFOV, 1f); 

            if (starMaterial != null)
            {
                starMaterial.SetFloat("_BrightnessMultiplier", brightnessMultiplier);
                starMaterial.SetFloat("_ZoomFactor", zoomFactor);
            }

            // render all the stars in one go
            if (starMaterial != null && starMesh != null)
            {
                Graphics.DrawMeshInstancedIndirect(
                    starMesh,
                    0,
                    starMaterial,
                    bounds,
                    argsBuffer,
                    0,
                    null,
                    UnityEngine.Rendering.ShadowCastingMode.Off,
                    false
                );
            }
        }

        void InitializeBuffers(int count)
        {
            positionBuffer = new ComputeBuffer(count, sizeof(float) * 3);

            scaleBuffer = new ComputeBuffer(count, sizeof(float) * 3);

            colorBuffer = new ComputeBuffer(count, sizeof(float) * 4);

            visibilityBuffer = new ComputeBuffer(count, sizeof(float));

            // args buffer for DrawMeshInstancedIndirect
            argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);

            if (starMesh != null)
            {
                args[0] = starMesh.GetIndexCount(0);
                args[1] = (uint)count;
                args[2] = starMesh.GetIndexStart(0);
                args[3] = starMesh.GetBaseVertex(0);
            }
            argsBuffer.SetData(args);

            // set buffers on material
            if (starMaterial != null)
            {
                starMaterial.SetBuffer("positionBuffer", positionBuffer);
                starMaterial.SetBuffer("scaleBuffer", scaleBuffer);
                starMaterial.SetBuffer("colorBuffer", colorBuffer);
                starMaterial.SetBuffer("visibilityBuffer", visibilityBuffer);
            }
        }

        void UpdateStarData(IReadOnlyList<Star> stars)
        {
            Vector3[] positions = new Vector3[currentStarCount];
            Vector3[] scales = new Vector3[currentStarCount];
            Vector4[] colors = new Vector4[currentStarCount];
            float[] visibility = new float[currentStarCount];

            Vector3 camPos = Camera.main != null ? Camera.main.transform.position : Vector3.zero;

            float currentFOV = Camera.main != null ? Camera.main.fieldOfView : 60f;
            float zoomFactor = 60f / Mathf.Max(currentFOV, 5f); // 
            float sizeScale = 1.5f / zoomFactor; // Shrink when zoomed in ( this doesnt work very well ) 

            for (int i = 0; i < currentStarCount; i++)
            {
                var star = stars[i];
                positions[i] = star.Position3D;

                Vector3 baseScale = star.LocalScale;
                float avgScale = (baseScale.x + baseScale.y + baseScale.z) / 3f;

                float uniformSize = 1.0f;
                float finalSize = Mathf.Lerp(uniformSize, avgScale, sizeVariation);

                //                finalSize *= sizeAdjust;

                scales[i] = new Vector3(finalSize, finalSize, finalSize) * sizeMultiplier * sizeScale;
                colors[i] = GetStarColor(star);

                float distToCam = Vector3.Distance(star.Position3D, camPos);
                bool tooClose = distToCam < minDistanceClamp;

                visibility[i] = (star.IsVisible && !tooClose) ? 1.0f : 0.0f;
            }

            positionBuffer.SetData(positions);
            scaleBuffer.SetData(scales);
            colorBuffer.SetData(colors);
            visibilityBuffer.SetData(visibility);
        }


        Vector4 GetStarColor(Star star)
        {
            float magnitude = star.Magnitude;


            float brightness = Mathf.Pow(2.512f, -magnitude + 1f);
            brightness = Mathf.Clamp(brightness, 0.02f, 5.0f);  

            Color starColor = GetStarColorFromSpectrum(star);

            return new Vector4(starColor.r, starColor.g, starColor.b, brightness);
        }

        // prob needs to be replaced with a table 
        Color GetStarColorFromSpectrum(Star star)
        {

            float magnitude = star.Magnitude;

            if (magnitude < 1.0f)
            {
                // blue-white
                return new Color(0.7f, 0.85f, 1.0f);
            }
            else if (magnitude < 2.5f)
            {
                //  white
                return new Color(1.0f, 1.0f, 1.0f);
            }
            else if (magnitude < 4.0f)
            {
                //yellow-white
                return new Color(1.0f, 0.95f, 0.8f);
            }
            else
            {
                // orange-red
                return new Color(1.0f, 0.8f, 0.6f);
            }
        }

        void ReleaseBuffers()
        {
            positionBuffer?.Release();
            scaleBuffer?.Release();
            colorBuffer?.Release();
            visibilityBuffer?.Release();
            argsBuffer?.Release();
        }

        void OnDestroy()
        {
            ReleaseBuffers();
        }

        Mesh CreateQuadMesh()
        {
            Mesh mesh = new Mesh();
            mesh.vertices = new Vector3[]
            {
                new Vector3(-0.5f, -0.5f, 0),
                new Vector3(0.5f, -0.5f, 0),
                new Vector3(-0.5f, 0.5f, 0),
                new Vector3(0.5f, 0.5f, 0)
            };
            mesh.uv = new Vector2[]
            {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(0, 1),
                new Vector2(1, 1)
            };
            mesh.triangles = new int[] { 0, 2, 1, 2, 3, 1 };
            mesh.RecalculateNormals();
            return mesh;
        }
    }
}