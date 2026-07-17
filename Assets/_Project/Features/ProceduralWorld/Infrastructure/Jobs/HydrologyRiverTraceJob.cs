using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using _Project.Features.ProceduralWorld.Infrastructure.Hydrology;
using _Project.Features.ProceduralWorld.Infrastructure.Jobs.Settings;
using Random = Unity.Mathematics.Random;

namespace _Project.Features.ProceduralWorld.Infrastructure.Jobs
{
    [BurstCompile]
    public struct HydrologyRiverTraceJob : IJob
    {
        [ReadOnly]
        public NativeArray<float> Heights;

        public int Size;

        public float OriginX;
        public float OriginZ;
        public float StepX;
        public float StepZ;

        public TerrainNoiseSettings Settings;

        [ReadOnly]
        public NativeArray<float2> Offsets;

        public HydrologySettings HydrologySettings;

        public int2 RegionCoord;

        public NativeList<float2Point> Points;

        public void Execute()
        {
            int seed =
                (int)math.hash(RegionCoord) ^
                HydrologySettings.Seed;

            Random random = new Random((uint)math.max(seed, 1));

            float average = ComputeAverageHeight();

            for (int river = 0; river < HydrologySettings.RiverSourceCount; river++)
            {
                int segmentId = unchecked(
                    (int)math.hash(new int3(RegionCoord.x, RegionCoord.y, river)));

                NativeList<float2Point> riverPoints = new NativeList<float2Point>(
                    HydrologySettings.MaxTraceSteps + 1,
                    Allocator.Temp);

                TraceRiver(ref random, riverPoints, segmentId, average);

                SmoothRiver(riverPoints, Points);

                riverPoints.Dispose();
            }
        }

        private float ComputeAverageHeight()
        {
            float average = 0f;

            for (int i = 0; i < Heights.Length; i++)
                average += Heights[i];

            return average / Heights.Length;
        }

        private void TraceRiver(
            ref Random random,
            NativeList<float2Point> points,
            int segmentId,
            float averageHeight)
        {
            int start = PickHighPoint(ref random, averageHeight);

            int sx = start % Size;
            int sz = start / Size;

            float2 pos = new float2(
                OriginX + sx * StepX,
                OriginZ + sz * StepZ);

            float height = HeightSampler.Sample(pos, Settings, Offsets);

            points.Add(new float2Point
            {
                X = pos.x,
                Z = pos.y,
                Height = height,
                Strength = HydrologySettings.LakeRadius,
                SegmentId = segmentId,
                Kind = HydrologyPointKind.Lake
            });

            float2 direction = float2.zero;
            float strength = 1f;
            float previousHeight = height;

            for (int i = 0; i < HydrologySettings.MaxTraceSteps; i++)
            {
                float terrain = HeightSampler.Sample(pos, Settings, Offsets);

                points.Add(new float2Point
                {
                    X = pos.x,
                    Z = pos.y,
                    Height = math.min(terrain, previousHeight),
                    Strength = strength,
                    SegmentId = segmentId,
                    Kind = HydrologyPointKind.River
                });

                float2 downhill = HeightSampler.SampleDownhillDirection(
                    pos,
                    HydrologySettings.GradientEpsilon,
                    Settings,
                    Offsets);

                if (math.lengthsq(downhill) < 1e-8f)
                    break;

                float2 desired =
                    math.lengthsq(direction) > 1e-8f
                        ? math.normalize(math.lerp(
                            direction,
                            downhill,
                            1f - HydrologySettings.TurnSmoothing))
                        : downhill;

                float wobble =
                    (random.NextFloat() - 0.5f) *
                    HydrologySettings.MeanderStrength;

                direction = math.normalize(
                    desired +
                    new float2(-desired.y, desired.x) * wobble);

                pos += direction * HydrologySettings.StepDistance;

                previousHeight = HeightSampler.Sample(pos, Settings, Offsets);

                strength = math.min(strength + 0.015f, 3f);
            }
        }

        private int PickHighPoint(ref Random random, float average)
        {
            for (int i = 0; i < 16; i++)
            {
                int id = random.NextInt(Heights.Length);

                if (Heights[id] > average)
                    return id;
            }

            return 0;
        }

        private static void SmoothRiver(
            NativeList<float2Point> riverPoints,
            NativeList<float2Point> output)
        {
            if (riverPoints.Length < 3)
            {
                for (int i = 0; i < riverPoints.Length; i++)
                    output.Add(riverPoints[i]);

                return;
            }

            for (int i = 0; i < riverPoints.Length - 1; i++)
            {
                float2Point a = riverPoints[i];
                float2Point b = riverPoints[i + 1];

                output.Add(a);

                if (a.Kind == HydrologyPointKind.Lake ||
                    b.Kind == HydrologyPointKind.Lake)
                {
                    continue;
                }

                output.Add(new float2Point
                {
                    X = math.lerp(a.X, b.X, .5f),
                    Z = math.lerp(a.Z, b.Z, .5f),
                    Height = math.lerp(a.Height, b.Height, .5f),
                    Strength = math.lerp(a.Strength, b.Strength, .5f),
                    SegmentId = a.SegmentId,
                    Kind = HydrologyPointKind.River
                });
            }

            output.Add(riverPoints[^1]);
        }
    }
}