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
        public float AccumulationThreshold;
        public float FalloffRange;
        public float MaxCarveDepth;

        public NativeArray<float> Heights;

        [WriteOnly] public NativeArray<float> RiverMask;
        [WriteOnly] public NativeArray<float> WaterSurfaceHeight;

        public void Execute(int index)
        {
            float accumulation = Accumulation[index];
            float originalHeight = Heights[index];
            
            WaterSurfaceHeight[index] = originalHeight;

            if (accumulation <= AccumulationThreshold)
            {
                RiverMask[index] = 0f;
                return;
            }

            float t = math.saturate((accumulation - AccumulationThreshold) / math.max(FalloffRange, 0.0001f));
            float smooth = t * t * (3f - 2f * t);

            float carved = originalHeight - smooth * MaxCarveDepth;
            Heights[index] = math.max(carved, 0f);

            RiverMask[index] = smooth;
        }
    }
}