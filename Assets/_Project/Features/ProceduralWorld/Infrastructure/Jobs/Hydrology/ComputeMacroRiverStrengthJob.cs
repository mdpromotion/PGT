using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace _Project.Features.ProceduralWorld.Infrastructure.Jobs.Hydrology
{
    [BurstCompile]
    public struct ComputeMacroRiverStrengthJob : IJobParallelFor
    {
        public float NormalizationRange;

        [ReadOnly] public NativeArray<float> Accumulation;
        [WriteOnly] public NativeArray<float> RiverStrengthRaw;

        public void Execute(int index)
        {
            float local = Accumulation[index];
            RiverStrengthRaw[index] = math.saturate(
                (local - 1f) / math.max(NormalizationRange, 0.0001f));
        }
    }
}