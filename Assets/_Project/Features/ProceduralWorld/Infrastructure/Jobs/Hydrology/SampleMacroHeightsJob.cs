using _Project.Features.ProceduralWorld.Infrastructure.Jobs.Landscape;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace _Project.Features.ProceduralWorld.Infrastructure.Jobs.Hydrology
{
    [BurstCompile]
    public struct SampleMacroHeightsJob : IJobParallelFor
    {
        public int PaddedSize;
        public float CellSize;
        public float2 WorldOrigin;

        public TerrainNoiseSettings Settings;
        [ReadOnly] public NativeArray<float2> OctaveOffsets;

        [WriteOnly] public NativeArray<float> Heights;

        public void Execute(int index)
        {
            int x = index % PaddedSize;
            int z = index / PaddedSize;

            float2 worldPos = WorldOrigin + new float2(x, z) * CellSize;

            Heights[index] = HeightSampler.Sample(worldPos, Settings, OctaveOffsets);
        }
    }
}