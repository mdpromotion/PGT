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
        
        [ReadOnly] public NativeArray<float> WaterSurfaceHeight;

        public int Resolution;

        public float AccumulationThreshold;
        public float FalloffRange;
        public float MaxCarveDepth;

        public NativeArray<float> Heights;

        [WriteOnly] public NativeArray<float> RiverMask;

        public void Execute(int index)
        {
            float originalHeight = Heights[index];
            float strength = Accumulation[index];
            float flatWaterLevel = WaterSurfaceHeight[index];

            float edgeStart = AccumulationThreshold;
            float edgeEnd = AccumulationThreshold + math.max(FalloffRange, 0.0001f);

            float mask = math.smoothstep(edgeStart, edgeEnd, strength);
            RiverMask[index] = mask;

            if (mask > 0f)
            {
                float targetBed = flatWaterLevel - MaxCarveDepth;
                
                float carvedHeight = math.max(math.lerp(originalHeight, targetBed, mask), 0f);
                Heights[index] = carvedHeight;
            }
        }
    }
}