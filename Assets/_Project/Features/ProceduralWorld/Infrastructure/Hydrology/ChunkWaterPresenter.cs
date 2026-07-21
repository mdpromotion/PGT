using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using _Project.Features.ProceduralWorld.Domain.Chunks;
using _Project.Features.ProceduralWorld.Domain.Landscape;

namespace _Project.Features.ProceduralWorld.Infrastructure.Hydrology
{
    public sealed class ChunkWaterPresenter
    {
        private static readonly int MaskHeightTexId =
            Shader.PropertyToID("_MaskHeightTex");

        private static readonly int HeightScaleId =
            Shader.PropertyToID("_HeightScale");

        private readonly float _heightScale;

        private MaterialPropertyBlock _propertyBlock;

        public ChunkWaterPresenter(
            float heightScale)
        {
            _heightScale = heightScale;
        }

        public void Apply(
            MeshRenderer waterRenderer,
            WaterState waterState,
            ChunkCoordinate coordinate,
            LandscapeData landscape)
        {
            if (waterRenderer == null || waterState == null)
                return;

            if (!landscape.RiverMask.IsCreated ||
                !landscape.WaterSurfaceHeight.IsCreated ||
                !landscape.BankHeight.IsCreated)
            {
                waterRenderer.enabled = false;
                return;
            }

            JobHandle handle = WaterSurfaceTextureBuilder.Schedule(
                landscape,
                out NativeArray<float4> pixels,
                out int outputRes);

            handle.Complete();

            waterState.Texture = WaterSurfaceTextureBuilder.Apply(
                waterState.Texture,
                pixels,
                outputRes);

            pixels.Dispose();

            _propertyBlock ??= new MaterialPropertyBlock();

            waterRenderer.GetPropertyBlock(_propertyBlock);

            _propertyBlock.SetTexture(MaskHeightTexId, waterState.Texture);
            _propertyBlock.SetFloat(HeightScaleId, _heightScale);

            waterRenderer.SetPropertyBlock(_propertyBlock);
            waterRenderer.enabled = true;
        }
    }
}