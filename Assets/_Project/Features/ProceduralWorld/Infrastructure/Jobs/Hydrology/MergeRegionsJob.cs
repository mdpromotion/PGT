using _Project.Features.ProceduralWorld.Infrastructure.Hydrology;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

namespace _Project.Features.ProceduralWorld.Infrastructure.Jobs.Hydrology
{
    [BurstCompile]
    public struct MergeRegionsJob : IJob
    {
        [ReadOnly] public NativeArray<float2Point> Source0;
        [ReadOnly] public NativeArray<float2Point> Source1;
        [ReadOnly] public NativeArray<float2Point> Source2;
        [ReadOnly] public NativeArray<float2Point> Source3;
        [ReadOnly] public NativeArray<float2Point> Source4;
        [ReadOnly] public NativeArray<float2Point> Source5;
        [ReadOnly] public NativeArray<float2Point> Source6;
        [ReadOnly] public NativeArray<float2Point> Source7;
        [ReadOnly] public NativeArray<float2Point> Source8;

        public NativeList<float2Point> Output;

        public void Execute()
        {
            Output.AddRange(Source0);
            Output.AddRange(Source1);
            Output.AddRange(Source2);
            Output.AddRange(Source3);
            Output.AddRange(Source4);
            Output.AddRange(Source5);
            Output.AddRange(Source6);
            Output.AddRange(Source7);
            Output.AddRange(Source8);
        }
    }
}