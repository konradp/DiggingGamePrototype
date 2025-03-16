using UnityEngine;

public class MeshDeformer : MonoBehaviour, IMeshDeformer
{
    [SerializeField] DeformerConfig config;
    
    private float[,] heightMap;
    private Mesh mesh;

    private void Start()
    {
        heightMap = new float[config.GridSize, config.GridSize];
        GenerateMesh();
    }

    private void GenerateMesh()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        int vertCount = (config.GridSize + 1) * (config.GridSize + 1);
        Vector3[] vertices = new Vector3[vertCount];
        Vector2[] uv = new Vector2[vertCount];
        int[] triangles = new int[config.GridSize * config.GridSize * 6];

        for (int z = 0, i = 0; z <= config.GridSize; z++)
        {
            for (int x = 0; x <= config.GridSize; x++, i++)
            {
                float height = (x < config.GridSize && z < config.GridSize) ? heightMap[x, z] : 0f;
                vertices[i] = new Vector3(x * config.CellSize, height, z * config.CellSize);
                uv[i] = new Vector2((float)x / config.GridSize, (float)z / config.GridSize);
            }
        }

        int tris = 0;
        for (int z = 0, vert = 0; z < config.GridSize; z++, vert++)
        {
            for (int x = 0; x < config.GridSize; x++, vert++)
            {
                int topLeft = vert;
                int bottomLeft = vert + config.GridSize + 1;
                int topRight = vert + 1;
                int bottomRight = vert + config.GridSize + 2;

                triangles[tris + 0] = topLeft;
                triangles[tris + 1] = bottomLeft;
                triangles[tris + 2] = topRight;

                triangles[tris + 3] = topRight;
                triangles[tris + 4] = bottomLeft;
                triangles[tris + 5] = bottomRight;

                tris += 6;
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        RecalculateTangents(mesh);

        var meshCollider = GetComponent<MeshCollider>();
        
        meshCollider.sharedMesh = null;
        meshCollider.sharedMesh = mesh;
    }
    
    private void RecalculateTangents(Mesh mesh)
    {
        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = mesh.normals;
        Vector2[] uv = mesh.uv;
        int[] triangles = mesh.triangles;

        Vector4[] tangents = new Vector4[vertices.Length];
        Vector3[] tan1 = new Vector3[vertices.Length];
        Vector3[] tan2 = new Vector3[vertices.Length];

        for (int i = 0; i < triangles.Length; i += 3)
        {
            int i1 = triangles[i];
            int i2 = triangles[i + 1];
            int i3 = triangles[i + 2];

            Vector3 v1 = vertices[i1];
            Vector3 v2 = vertices[i2];
            Vector3 v3 = vertices[i3];

            Vector2 w1 = uv[i1];
            Vector2 w2 = uv[i2];
            Vector2 w3 = uv[i3];

            float x1 = v2.x - v1.x;
            float x2 = v3.x - v1.x;
            float y1 = v2.y - v1.y;
            float y2 = v3.y - v1.y;
            float z1 = v2.z - v1.z;
            float z2 = v3.z - v1.z;

            float s1 = w2.x - w1.x;
            float s2 = w3.x - w1.x;
            float t1 = w2.y - w1.y;
            float t2 = w3.y - w1.y;

            float r = 1.0f / (s1 * t2 - s2 * t1);
            Vector3 sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
            Vector3 tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);

            tan1[i1] += sdir;
            tan1[i2] += sdir;
            tan1[i3] += sdir;

            tan2[i1] += tdir;
            tan2[i2] += tdir;
            tan2[i3] += tdir;
        }

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 n = normals[i];
            Vector3 t = tan1[i];

            Vector3.OrthoNormalize(ref n, ref t);
            tangents[i] = new Vector4(t.x, t.y, t.z, (Vector3.Dot(Vector3.Cross(n, t), tan2[i]) < 0.0f) ? -1.0f : 1.0f);
        }

        mesh.tangents = tangents;
    }

    public void Deform(Vector3 worldPoint)
    {
        Vector3 localPoint = transform.InverseTransformPoint(worldPoint);

        int xStart = Mathf.Max(0, Mathf.FloorToInt(localPoint.x / config.CellSize - config.DeformationRadius));
        int zStart = Mathf.Max(0, Mathf.FloorToInt(localPoint.z / config.CellSize - config.DeformationRadius));
        int xEnd = Mathf.Min(config.GridSize, Mathf.CeilToInt(localPoint.x / config.CellSize + config.DeformationRadius));
        int zEnd = Mathf.Min(config.GridSize, Mathf.CeilToInt(localPoint.z / config.CellSize + config.DeformationRadius));

        for (int z = zStart; z <= zEnd; z++)
        {
            for (int x = xStart; x <= xEnd; x++)
            {
                float dist = Vector3.Distance(new Vector3(x * config.CellSize, 0, z * config.CellSize), new Vector3(localPoint.x, 0, localPoint.z));
                if (dist < config.DeformationRadius)
                {
                    heightMap[x, z] -= (1 - (dist / config.DeformationRadius)) * config.DeformationDepth;
                }
            }
        }

        GenerateMesh();
    }
}

public interface IMeshDeformer
{
    public void Deform(Vector3 worldPoint);
}
