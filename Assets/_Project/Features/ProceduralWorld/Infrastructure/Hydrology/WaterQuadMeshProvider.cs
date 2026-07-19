using UnityEngine;

namespace _Project.Features.ProceduralWorld.Infrastructure.Hydrology
{
    public static class WaterQuadMeshProvider
    {
        private static Mesh _shared;
        private static float _cachedSizeX;
        private static float _cachedSizeZ;
        private static int _cachedSubdivisions;

        public static Mesh Get(
            float chunkSizeX,
            float chunkSizeZ,
            int subdivisions = 32)
        {
            if (_shared != null &&
                Mathf.Approximately(_cachedSizeX, chunkSizeX) &&
                Mathf.Approximately(_cachedSizeZ, chunkSizeZ) &&
                _cachedSubdivisions == subdivisions)
            {
                return _shared;
            }

            _shared = Build(chunkSizeX, chunkSizeZ, subdivisions);
            _cachedSizeX = chunkSizeX;
            _cachedSizeZ = chunkSizeZ;
            _cachedSubdivisions = subdivisions;

            return _shared;
        }

        private static Mesh Build(
            float sizeX,
            float sizeZ,
            int subdivisions)
        {
            int verticesPerSide = subdivisions + 1;
            int vertexCount = verticesPerSide * verticesPerSide;

            Vector3[] vertices = new Vector3[vertexCount];
            Vector2[] uvs = new Vector2[vertexCount];
            int[] triangles = new int[subdivisions * subdivisions * 6];

            float stepX = sizeX / subdivisions;
            float stepZ = sizeZ / subdivisions;

            int vertexIndex = 0;

            for (int z = 0; z < verticesPerSide; z++)
            {
                for (int x = 0; x < verticesPerSide; x++)
                {
                    vertices[vertexIndex] = new Vector3(x * stepX, 0f, z * stepZ);
                    uvs[vertexIndex] = new Vector2(
                        x / (float)subdivisions,
                        z / (float)subdivisions);

                    vertexIndex++;
                }
            }

            int triangleIndex = 0;

            for (int z = 0; z < subdivisions; z++)
            {
                for (int x = 0; x < subdivisions; x++)
                {
                    int i0 = z * verticesPerSide + x;
                    int i1 = i0 + 1;
                    int i2 = i0 + verticesPerSide;
                    int i3 = i2 + 1;

                    triangles[triangleIndex++] = i0;
                    triangles[triangleIndex++] = i2;
                    triangles[triangleIndex++] = i1;

                    triangles[triangleIndex++] = i1;
                    triangles[triangleIndex++] = i2;
                    triangles[triangleIndex++] = i3;
                }
            }

            Mesh mesh = new Mesh
            {
                name = "SharedWaterQuad",
                vertices = vertices,
                uv = uvs,
                triangles = triangles
            };

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            
            mesh.bounds = new Bounds(
                new Vector3(sizeX * 0.5f, 0f, sizeZ * 0.5f),
                new Vector3(sizeX, 1000f, sizeZ));

            return mesh;
        }
    }
}