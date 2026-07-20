using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using _Project.Features.ProceduralWorld.Domain.Landscape;

namespace _Project.Features.ProceduralWorld.Infrastructure.Hydrology
{
    [BurstCompile]
    public struct BuildWaterSurfaceTextureJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<float> Mask;
        [ReadOnly] public NativeArray<float> Height;
        [ReadOnly] public NativeArray<float> Bank;

        public int SourceRes;
        public int OutputRes;
        public float Scale;

        [WriteOnly]
        public NativeArray<float4> Pixels;

        public void Execute(int index)
        {
            int x = index % OutputRes;
            int z = index / OutputRes;

            float sx = x * Scale;
            float sz = z * Scale;

            float m = LandscapeSampler.SampleBilinear(Mask, SourceRes, sx, sz);
            float h = LandscapeSampler.SampleBilinear(Height, SourceRes, sx, sz);
            float bk = LandscapeSampler.SampleBilinear(Bank, SourceRes, sx, sz);

            Pixels[index] = new float4(math.saturate(m), h, bk, 0f);
        }
    }

    public static class WaterSurfaceTextureBuilder
    {
        public static JobHandle Schedule(
            LandscapeData landscape,
            out NativeArray<float4> pixels,
            out int outputRes)
        {
            int sourceRes = landscape.Resolution;
            outputRes = sourceRes;

            pixels = new NativeArray<float4>(
                outputRes * outputRes,
                Allocator.Persistent,
                NativeArrayOptions.UninitializedMemory);

            float scale = (sourceRes - 1) / (float)(outputRes - 1);

            BuildWaterSurfaceTextureJob job = new BuildWaterSurfaceTextureJob
            {
                Mask = landscape.RiverMask,
                Height = landscape.WaterSurfaceHeight,
                Bank = landscape.BankHeight,
                SourceRes = sourceRes,
                OutputRes = outputRes,
                Scale = scale,
                Pixels = pixels
            };

            return job.Schedule(outputRes * outputRes, 64);
        }
        
        public static Texture2D Apply(
            Texture2D existing,
            NativeArray<float4> pixels,
            int outputRes)
        {
            Texture2D texture = existing;

            if (texture == null ||
                texture.width != outputRes ||
                texture.height != outputRes)
            {
                if (texture != null)
                {
                    Object.Destroy(texture);
                }

                texture = new Texture2D(
                    outputRes,
                    outputRes,
                    TextureFormat.RGBAFloat,
                    mipChain: false,
                    linear: true)
                {
                    wrapMode = TextureWrapMode.Clamp,
                    filterMode = FilterMode.Bilinear
                };
            }

            texture.SetPixelData(pixels, 0);
            texture.Apply(updateMipmaps: false, makeNoLongerReadable: false);

            return texture;
        }
    }
}