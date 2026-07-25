using System.Collections.Generic;
using Unity.Collections;
using _Project.Features.ProceduralWorld.Domain.Chunks;
using UnityEngine;

namespace _Project.Features.ProceduralWorld.Infrastructure.Hydrology
{
    public sealed class WaterSurfaceApplier
    {
        private static readonly int MaskHeightTexId = Shader.PropertyToID("_MaskHeightTex");

        public void Apply(Terrain terrain, ChunkGenerationState state)
        {
            Transform waterTransform = terrain.transform.Find("Water");
            if (waterTransform == null)
            {
                DisposeTransient(state);
                return;
            }

            bool hasWater = state.WaterBounds.IsCreated && state.WaterBounds[0] != 0;

            if (!hasWater)
            {
                waterTransform.gameObject.SetActive(false);
                DisposeTransient(state);
                return;
            }

            waterTransform.gameObject.SetActive(true);

            Renderer renderer = waterTransform.GetComponent<Renderer>();
            MeshFilter meshFilter = waterTransform.GetComponent<MeshFilter>();

            if (renderer == null || meshFilter == null)
            {
                DisposeTransient(state);
                return;
            }

            int resolution = state.Context.Resolution;

            var texture = new Texture2D(resolution, resolution, TextureFormat.RGBA32, false, true)
            {
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Bilinear
            };
            texture.SetPixelData(state.WaterMaskPixels, 0);
            texture.Apply(false, true);
            
            Mesh mesh = BuildWaterMesh(terrain, state, resolution);
            meshFilter.sharedMesh = mesh;

            waterTransform.localPosition = Vector3.zero;
            waterTransform.localRotation = Quaternion.identity;
            waterTransform.localScale = Vector3.one;

            var block = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(block);
            block.SetTexture(MaskHeightTexId, texture);
            renderer.SetPropertyBlock(block);

            DisposeTransient(state);
        }

        private static Mesh BuildWaterMesh(Terrain terrain, ChunkGenerationState state, int resolution)
        {
            NativeArray<int> bounds = state.WaterBounds;
            int minX = bounds[1], minZ = bounds[2], maxX = bounds[3], maxZ = bounds[4];

            int gridW = maxX - minX + 1;
            int gridH = maxZ - minZ + 1;

            Vector3 size = terrain.terrainData.size;
            float invRes = 1f / (resolution - 1);

            var vertices = new Vector3[gridW * gridH];
            var uvs = new Vector2[gridW * gridH];
            var indices = new List<int>();

            NativeArray<float> surfaceHeight = state.Hydrology.WaterSurfaceHeight;
            NativeArray<float> riverMask = state.Hydrology.RiverMask;

            for (int z = 0; z < gridH; z++)
            {
                for (int x = 0; x < gridW; x++)
                {
                    int sampleX = minX + x;
                    int sampleZ = minZ + z;
                    int sampleIndex = sampleZ * resolution + sampleX;

                    float u = sampleX * invRes;
                    float v = sampleZ * invRes;

                    float h = surfaceHeight[sampleIndex] * size.y;

                    int vIndex = z * gridW + x;
                    vertices[vIndex] = new Vector3(u * size.x, h, v * size.z);
                    uvs[vIndex] = new Vector2(u, v);
                }
            }
            
            for (int z = 0; z < gridH - 1; z++)
            {
                for (int x = 0; x < gridW - 1; x++)
                {
                    int i00 = z * gridW + x;
                    int i10 = z * gridW + x + 1;
                    int i01 = (z + 1) * gridW + x;
                    int i11 = (z + 1) * gridW + x + 1;

                    indices.Add(i00); indices.Add(i01); indices.Add(i10);
                    indices.Add(i01); indices.Add(i11); indices.Add(i10);
                }
            }

            var mesh = new Mesh { name = "WaterSurface" };
            mesh.SetVertices(vertices);
            mesh.SetUVs(0, uvs);
            mesh.SetTriangles(indices, 0);
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();

            return mesh;
        }

        private static void DisposeTransient(ChunkGenerationState state)
        {
            if (state.WaterMaskPixels.IsCreated) state.WaterMaskPixels.Dispose();
            if (state.WaterBounds.IsCreated) state.WaterBounds.Dispose();
            if (state.WaterAverageHeight.IsCreated) state.WaterAverageHeight.Dispose();
        }
    }
}