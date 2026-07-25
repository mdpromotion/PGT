using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace _Project.Features.ProceduralWorld.Infrastructure.Jobs.Hydrology
{
    [BurstCompile]
    public struct PackWaterSurfaceTextureJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<float> RiverMask;
        [ReadOnly] public NativeArray<float> WaterSurfaceHeight;

        [WriteOnly] public NativeArray<Color32> MaskPixels;

        public void Execute(int index)
        {
            float mask = RiverMask[index];
            float height = WaterSurfaceHeight[index];

            byte r = (byte)(math.saturate(mask) * 255f);
            byte g = (byte)(math.saturate(height) * 255f);

            MaskPixels[index] = new Color32(r, g, 0, 255);
        }
    }
}