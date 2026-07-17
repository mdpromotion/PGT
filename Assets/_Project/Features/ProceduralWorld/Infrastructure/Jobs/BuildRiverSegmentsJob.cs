using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using _Project.Features.ProceduralWorld.Infrastructure.Hydrology;

namespace _Project.Features.ProceduralWorld.Infrastructure.Jobs
{
    [BurstCompile(FloatMode = FloatMode.Fast, FloatPrecision = FloatPrecision.Low)]
    public struct BuildRiverSegmentsJob : IJob
    {
        [ReadOnly]
        public NativeArray<float2Point> Points;

        public NativeList<RiverSegment> Segments;

        public void Execute()
        {
            Segments.Clear();

            int count = Points.Length;
            if (count < 2)
                return;

            for (int i = 0; i < count - 1; i++)
            {
                float2Point a = Points[i];
                float2Point b = Points[i + 1];

                if (a.Kind != HydrologyPointKind.River ||
                    b.Kind != HydrologyPointKind.River ||
                    a.SegmentId != b.SegmentId)
                {
                    continue;
                }

                Segments.Add(new RiverSegment
                {
                    A = new float2(a.X, a.Z),
                    B = new float2(b.X, b.Z),
                    HeightA = a.Height,
                    HeightB = b.Height,
                    StrengthA = a.Strength,
                    StrengthB = b.Strength,
                    SegmentId = a.SegmentId
                });
            }
        }
    }
}