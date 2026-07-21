using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

namespace _Project.Features.ProceduralWorld.Infrastructure.Jobs.Hydrology
{
    [BurstCompile]
    public struct FillFloatJob : IJobParallelFor
    {
        [WriteOnly]
        public NativeArray<float> Values;

        public float Value;

        public void Execute(int index) => Values[index] = Value;
    }
}