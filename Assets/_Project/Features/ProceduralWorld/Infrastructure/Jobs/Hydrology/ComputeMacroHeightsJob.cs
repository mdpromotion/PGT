using _Project.Features.ProceduralWorld.Infrastructure.Jobs.Settings;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace _Project.Features.ProceduralWorld.Infrastructure.Jobs.Hydrology
{
    [BurstCompile]
    public struct ComputeMacroHeightsJob : IJobParallelFor
    {
        [ReadOnly] public TerrainNoiseSettings Settings;
        [ReadOnly] public NativeArray<float2> OctaveOffsets;

        public int Resolution;
        public float2 CellSize;
        public float2 OriginWorld;

        [WriteOnly] public NativeArray<float> Heights;

        public void Execute(int index)
        {
            int x = index % Resolution;
            int y = index / Resolution;

            float2 worldPos = OriginWorld + new float2(x, y) * CellSize;
            Heights[index] = HeightSampler.Sample(worldPos, Settings, OctaveOffsets);
        }
    }
}