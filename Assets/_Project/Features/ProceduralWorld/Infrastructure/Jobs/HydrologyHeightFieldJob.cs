using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using _Project.Features.ProceduralWorld.Infrastructure.Jobs.Settings;
using Unity.Jobs;

namespace _Project.Features.ProceduralWorld.Infrastructure.Jobs
{
    [BurstCompile]
    public struct HydrologyHeightFieldJob : IJobParallelFor
    {
        [WriteOnly]
        public NativeArray<float> Heights;

        public int Size;

        public float OriginX;
        public float OriginZ;
        public float StepX;
        public float StepZ;

        public TerrainNoiseSettings Settings;

        [ReadOnly]
        public NativeArray<float2> Offsets;

        public void Execute(int index)
        {
            int x = index % Size;
            int z = index / Size;

            float2 pos = new float2(
                OriginX + x * StepX,
                OriginZ + z * StepZ);

            Heights[index] = HeightSampler.Sample(pos, Settings, Offsets);
        }
    }
}