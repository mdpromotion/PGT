using Unity.Collections;
using UnityEngine;

namespace _Project.Features.ProceduralWorld.Infrastructure.Hydrology
{
    public static class WaterMeshBuilder
    {
        public static void Build(
            Mesh mesh,
            NativeArray<float> waterSurfaceHeight,
            int resolution,
            float chunkSizeX,
            float chunkSizeZ,
            float heightScale,
            int stride)
        {
            int gridSize = (resolution - 1) / stride + 1;
            int vertexCount = gridSize * gridSize;

            var vertices = new Vector3[vertexCount];
            var uvs = new Vector2[vertexCount];
            var normals = new Vector3[vertexCount];

            float cellWorldX = chunkSizeX / (resolution - 1);
            float cellWorldZ = chunkSizeZ / (resolution - 1);

            for (int gz = 0; gz < gridSize; gz++)
            {
                int fz = gz * stride;

                for (int gx = 0; gx < gridSize; gx++)
                {
                    int fx = gx * stride;

                    int fineIndex = fz * resolution + fx;
                    int vertIndex = gz * gridSize + gx;

                    float height = waterSurfaceHeight[fineIndex] * heightScale;

                    vertices[vertIndex] = new Vector3(fx * cellWorldX, height, fz * cellWorldZ);
                    
                    uvs[vertIndex] = new Vector2(
                        (float)fx / (resolution - 1),
                        (float)fz / (resolution - 1));

                    normals[vertIndex] = Vector3.up;
                }
            }

            int quadCount = (gridSize - 1) * (gridSize - 1);
            var triangles = new int[quadCount * 6];

            int t = 0;
            for (int gz = 0; gz < gridSize - 1; gz++)
            {
                for (int gx = 0; gx < gridSize - 1; gx++)
                {
                    int i00 = gz * gridSize + gx;
                    int i10 = gz * gridSize + gx + 1;
                    int i01 = (gz + 1) * gridSize + gx;
                    int i11 = (gz + 1) * gridSize + gx + 1;

                    triangles[t++] = i00;
                    triangles[t++] = i01;
                    triangles[t++] = i10;

                    triangles[t++] = i10;
                    triangles[t++] = i01;
                    triangles[t++] = i11;
                }
            }

            mesh.Clear();
            mesh.indexFormat = vertexCount > 65000
                ? UnityEngine.Rendering.IndexFormat.UInt32
                : UnityEngine.Rendering.IndexFormat.UInt16;

            mesh.vertices = vertices;
            mesh.uv = uvs;
            mesh.normals = normals;
            mesh.triangles = triangles;
            mesh.RecalculateBounds();
        }
    }
}