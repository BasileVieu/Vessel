using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshCollider))]
public class WaterCollider : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public Vector2 direction = Vector2.right;
        public float steepness = 0.5f;
        public float wavelength = 10.0f;
        public float speed = 1.0f;
    }

    [SerializeField] private Transform target;

    [SerializeField] private int xSize = 10;
    [SerializeField] private int zSize = 10;

    [SerializeField] private List<Wave> waves = new List<Wave>();

    private MeshCollider meshCollider;
    private Mesh mesh;

    private Vector3[] originalPositions;

    private Matrix4x4 localToWorld;

    private void Awake()
    {
        meshCollider = GetComponent<MeshCollider>();

        Generate();
    }

    private void FixedUpdate()
    {
        transform.position = new Vector3(target.position.x, 0.0f, target.position.z);
        
        localToWorld = transform.localToWorldMatrix;

        Vector3[] vertices = meshCollider.sharedMesh.vertices;

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 position = localToWorld.MultiplyPoint3x4(new Vector3(originalPositions[i].x, 0.0f, originalPositions[i].z));

            Vector3 p = position;

            foreach (Wave wave in waves)
            {
                p.y += GetGerstnerWavePosition(position, wave.direction, wave.speed, wave.steepness, wave.wavelength);
            }

            vertices[i] = p - transform.position;
        }

        mesh.SetVertices(vertices);

        meshCollider.sharedMesh = mesh;
    }

    private void Generate()
    {
        meshCollider.sharedMesh = mesh = new Mesh();
        mesh.name = "Mesh";
        mesh.MarkDynamic();

        Vector3[] vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        Vector2[] uv = new Vector2[vertices.Length];

        Vector4[] tangents = new Vector4[vertices.Length];

        Vector4 tangent = new Vector4(1.0f, 0.0f, 0.0f, -1.0f);

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++, i++)
            {
                vertices[i] = new Vector3(x - xSize / 2.0f, 0.0f, z - zSize / 2.0f);

                uv[i] = new Vector2((float)x / xSize, (float)z / zSize);

                tangents[i] = tangent;
            }
        }

        mesh.vertices = originalPositions = vertices;
        mesh.uv = uv;
        mesh.tangents = tangents;

        int[] triangles = new int[xSize * zSize * 6];

        for (int ti = 0, vi = 0, z = 0; z < zSize; z++, vi++)
        {
            for (int x = 0; x < xSize; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
                triangles[ti + 5] = vi + xSize + 2;
            }
        }

        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        meshCollider.convex = false;
    }

    float GetGerstnerWavePosition(Vector3 _position, Vector2 _direction, float _speed, float _steepness, float _wavelength)
    {
        float k = 2 * Mathf.PI / _wavelength;
        float c = Mathf.Sqrt(Mathf.Abs(Physics.gravity.y) / k) * _speed;

        Vector2 d = _direction.normalized;

        float dot = Vector2.Dot(d, new Vector2(_position.x, _position.z));

        float f = k * (dot - c * Time.timeSinceLevelLoad);
        float a = _steepness / k;

        return a * Mathf.Sin(f);
    }
}