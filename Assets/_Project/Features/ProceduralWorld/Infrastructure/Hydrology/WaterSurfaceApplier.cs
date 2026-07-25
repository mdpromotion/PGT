using _Project.Features.ProceduralWorld.Application.Chunks.Generation;
using _Project.Features.ProceduralWorld.Domain;
using _Project.Features.ProceduralWorld.Domain.Chunks;
using UnityEngine;

namespace _Project.Features.ProceduralWorld.Infrastructure.Hydrology
{
    public sealed class WaterSurfaceApplier
    {
        private static readonly int MaskHeightTexId = Shader.PropertyToID("_MaskHeightTex");
        private const string WaterChildName = "Water";

        private readonly ChunkGrid _grid;
        private readonly float _heightScale;
        private readonly Material _sharedMaterial;
        private readonly int _meshStride;

        public WaterSurfaceApplier(
            ChunkGrid grid,
            float heightScale,
            Material sharedMaterial,
            int meshStride = 4)
        {
            _grid = grid;
            _heightScale = heightScale;
            _sharedMaterial = sharedMaterial;
            _meshStride = meshStride;
        }
        
        public void Apply(ChunkGenerationState state, Transform chunkRoot)
        {
            Transform water = chunkRoot.Find(WaterChildName);
            if (water == null)
            {
                Debug.LogError(
                    $"WaterSurfaceApplier: '{chunkRoot.name}' has no child named '{WaterChildName}'.");
                return;
            }

            bool hasWater =
                state.WaterBounds.IsCreated &&
                state.WaterBounds.Length > 0 &&
                state.WaterBounds[0] == 1;

            water.gameObject.SetActive(hasWater);

            if (hasWater)
                UpdateWaterSurface(water, state);
        }

        private void UpdateWaterSurface(Transform water, ChunkGenerationState state)
        {
            MeshFilter filter = water.GetComponent<MeshFilter>();
            if (filter == null)
            {
                Debug.LogError($"WaterSurfaceApplier: '{WaterChildName}' has no MeshFilter.");
                return;
            }

            Mesh mesh = filter.sharedMesh;
            if (mesh == null || mesh.name != "WaterSurface")
            {
                mesh = new Mesh { name = "WaterSurface" };
                filter.sharedMesh = mesh;
            }

            WaterMeshBuilder.Build(
                mesh,
                state.Hydrology.WaterSurfaceHeight,
                state.Context.Resolution,
                _grid.ChunkSizeX,
                _grid.ChunkSizeZ,
                _heightScale,
                _meshStride);

            var texture = new Texture2D(
                state.Context.Resolution,
                state.Context.Resolution,
                TextureFormat.RGBA32,
                mipChain: false,
                linear: true)
            {
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Bilinear,
            };

            texture.SetPixelData(state.WaterMaskPixels, 0);
            texture.Apply(updateMipmaps: false, makeNoLongerReadable: true);

            var renderer = water.GetComponent<Renderer>();
            if (renderer.sharedMaterial == null)
                renderer.sharedMaterial = _sharedMaterial;

            var block = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(block);
            block.SetTexture(MaskHeightTexId, texture);
            renderer.SetPropertyBlock(block);
        }

        private static void DisposeTransient(ChunkGenerationState state)
        {
        }
    }
}