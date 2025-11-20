using UnityEngine;

public class GPUInstanceTest : MonoBehaviour
{ 
    [Header("Settings")]
    public int instanceCount = 1000;
    public Material instanceMaterial;
    public Mesh instanceMesh;

    private ComputeBuffer positionBuffer;
    private ComputeBuffer argsBuffer;
    private uint[] args = new uint[5] { 0, 0, 0, 0, 0 };
    private Bounds bounds;

    void Start()
    {
        if (instanceMesh == null)
        {
            instanceMesh = CreateQuadMesh();
        }

        bounds = new Bounds(Vector3.zero, Vector3.one * 1000f);

        positionBuffer = new ComputeBuffer(instanceCount, sizeof(float) * 3);

        Vector3[] positions = new Vector3[instanceCount];
        for (int i = 0; i < instanceCount; i++)
        {
            positions[i] = Random.insideUnitSphere * 50f;
        }
        positionBuffer.SetData(positions);

        argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        if (instanceMesh != null)
        {
            args[0] = instanceMesh.GetIndexCount(0);
            args[1] = (uint)instanceCount;
            args[2] = instanceMesh.GetIndexStart(0);
            args[3] = instanceMesh.GetBaseVertex(0);
        }
        argsBuffer.SetData(args);

        if (instanceMaterial != null)
        {
            instanceMaterial.SetBuffer("positionBuffer", positionBuffer);
        }
    }

    void Update()
    {
        if (instanceMaterial != null && instanceMesh != null)
        {
            Graphics.DrawMeshInstancedIndirect(instanceMesh, 0, instanceMaterial, bounds, argsBuffer);
        }
    }

    void OnDestroy()
    {
        if (positionBuffer != null) positionBuffer.Release();
        if (argsBuffer != null) argsBuffer.Release();
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