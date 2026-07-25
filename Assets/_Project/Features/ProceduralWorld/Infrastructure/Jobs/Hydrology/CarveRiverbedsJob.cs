using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace _Project.Features.ProceduralWorld.Infrastructure.Jobs.Hydrology
{
    [BurstCompile]
    public struct CarveRiverbedsJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<float> Accumulation;

        public int Resolution;

        public float AccumulationThreshold;
        public float FalloffRange;
        public float MaxCarveDepth;

        public NativeArray<float> Heights;

        [WriteOnly] public NativeArray<float> RiverMask;
        [WriteOnly] public NativeArray<float> WaterSurfaceHeight;

        public void Execute(int index)
        {
            float originalHeight = Heights[index];
            WaterSurfaceHeight[index] = originalHeight;

            float strength = Accumulation[index];

            float edgeStart = AccumulationThreshold;
            float edgeEnd = AccumulationThreshold + math.max(FalloffRange, 0.0001f);

            float mask = math.smoothstep(edgeStart, edgeEnd, strength);

            RiverMask[index] = mask;

            if (mask > 0f)
            {
                Heights[index] = math.max(originalHeight - mask * MaxCarveDepth, 0f);
            }
        }
    }
}