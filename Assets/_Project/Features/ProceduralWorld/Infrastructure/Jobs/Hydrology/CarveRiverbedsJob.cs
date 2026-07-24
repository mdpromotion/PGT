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

        public void Execute(int index)
        {
            float accumulation = Accumulation[index];
            if (accumulation <= AccumulationThreshold)
                return;

            float t = math.saturate((accumulation - AccumulationThreshold) / math.max(FalloffRange, 0.0001f));
            float smooth = t * t * (3f - 2f * t);

            float carved = Heights[index] - smooth * MaxCarveDepth;
            Heights[index] = math.max(carved, 0f);
        }
    }
}