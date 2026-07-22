using _Project.Features.ProceduralWorld.Infrastructure.Hydrology;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace _Project.Features.ProceduralWorld.Infrastructure.Jobs.Hydrology
{
    [BurstCompile]
    public struct DuplicatePointsJob : IJob
    {
        [ReadOnly]
        public NativeList<float2Point> Source;

        public NativeList<float2Point> Dest;

        public void Execute()
        {
            Dest.Clear();
            Dest.AddRange(Source.AsArray());
        }
    }

    [BurstCompile]
    public struct MinFilterRiverHeightJob : IJobParallelForDefer
    {
        [ReadOnly]
        public NativeArray<float2Point> Source;

        public NativeArray<float2Point> Points;

        public int WindowRadius;
        public float SafetyOffset;

        public void Execute(int i)
        {
            float2Point p = Points[i];

            if (p.Kind != HydrologyPointKind.River)
                return;

            int segId = p.SegmentId;
            float minHeight = p.Height;

            int lo = math.max(0, i - WindowRadius);
            int hi = math.min(Source.Length - 1, i + WindowRadius);

            for (int j = lo; j <= hi; j++)
            {
                float2Point n = Source[j];

                if (n.SegmentId != segId || n.Kind != HydrologyPointKind.River)
                    continue;

                minHeight = math.min(minHeight, n.Height);
            }

            p.Height = minHeight - SafetyOffset;
            Points[i] = p;
        }
    }
}