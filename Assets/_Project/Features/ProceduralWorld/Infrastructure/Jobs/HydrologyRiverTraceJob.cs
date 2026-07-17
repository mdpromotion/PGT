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
        
        private int PickHighPoint(ref Random random, float averageHeight, out bool isBasin)
        {
            int bestId = -1;
            float bestHeight = averageHeight;
            isBasin = false;

            for (int attempt = 0; attempt < 24; attempt++)
            {
                int id = random.NextInt(Heights.Length);

                int x = id % Size;
                int z = id / Size;

                if (x < 2 || z < 2 || x >= Size - 2 || z >= Size - 2)
                    continue;

                float h = Heights[id];

                if (h <= bestHeight)
                    continue;

                bool isLocalMax = true;

                for (int dz = -1; dz <= 1 && isLocalMax; dz++)
                {
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        if (dx == 0 && dz == 0)
                            continue;

                        int neighbourId = (z + dz) * Size + (x + dx);

                        if (Heights[neighbourId] > h)
                        {
                            isLocalMax = false;
                            break;
                        }
                    }
                }

                if (!isLocalMax)
                    continue;

                bestHeight = h;
                bestId = id;
            }

            if (bestId >= 0)
            {
                int bx = bestId % Size;
                int bz = bestId / Size;

                float minH = float.MaxValue;
                float maxH = float.MinValue;

                for (int dz = -2; dz <= 2; dz++)
                {
                    for (int dx = -2; dx <= 2; dx++)
                    {
                        int nx = bx + dx;
                        int nz = bz + dz;

                        int neighbourId = nz * Size + nx;
                        float h = Heights[neighbourId];

                        minH = math.min(minH, h);
                        maxH = math.max(maxH, h);
                    }
                }

                isBasin = (maxH - minH) <= HydrologySettings.SpringBasinFlatness;
            }

            return bestId >= 0 ? bestId : 0;
        }

        private void TraceRiver(
            ref Random random,
            NativeList<float2Point> points,
            int segmentId,
            float averageHeight)
        {
            int start = PickHighPoint(ref random, averageHeight, out bool isBasin);

            int sx = start % Size;
            int sz = start / Size;

            float2 pos = new float2(
                OriginX + sx * StepX,
                OriginZ + sz * StepZ);

            float height = HeightSampler.Sample(pos, Settings, Offsets);
            
            if (isBasin)
            {
                points.Add(new float2Point
                {
                    X = pos.x,
                    Z = pos.y,
                    Height = height,
                    Strength = HydrologySettings.LakeRadius,
                    SegmentId = segmentId,
                    Kind = HydrologyPointKind.Lake
                });
            }

            float2 direction = float2.zero;
            float strength = HydrologySettings.InitialRiverStrength;
            float previousHeight = height;

            int stepsTaken = 0;
            bool endedByConfluence = false;

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

                stepsTaken++;

                if (TryFindConfluence(pos, segmentId, out int hitIndex, out bool hitLake))
                {
                    float2Point hitPoint = Points[hitIndex];

                    float2Point last = points[points.Length - 1];
                    last.X = hitPoint.X;
                    last.Z = hitPoint.Z;
                    points[points.Length - 1] = last;

                    if (!hitLake)
                    {
                        BoostDownstream(
                            Points,
                            hitIndex,
                            hitPoint.SegmentId,
                            math.min(
                                strength * HydrologySettings.ConfluenceStrengthFactor,
                                HydrologySettings.MaxRiverStrength));
                    }

                    endedByConfluence = true;
                    break;
                }

                float slope;

                float2 downhill = HeightSampler.SampleDownhillDirection(
                    pos,
                    HydrologySettings.GradientEpsilon,
                    Settings,
                    Offsets,
                    out slope);

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

                float targetStrength = math.min(
                    strength +
                    HydrologySettings.BaseStrengthGrowth +
                    slope * HydrologySettings.SlopeStrengthFactor,
                    HydrologySettings.MaxRiverStrength);
                
                if (stepsTaken < HydrologySettings.RiverStartFadeSteps)
                {
                    float t = stepsTaken / (float)HydrologySettings.RiverStartFadeSteps;
                    t = t * t * (3f - 2f * t);

                    strength = math.lerp(
                        HydrologySettings.InitialRiverStrength,
                        targetStrength,
                        t);
                }
                else
                {
                    strength = targetStrength;
                }
            }

            if (!endedByConfluence)
            {
                ApplyEndFade(points);
            }
        }

        private static void ApplyEndFade(NativeList<float2Point> points)
        {
            int fadeSteps = 0;
            int count = points.Length;
            
            for (int i = count - 1; i >= 0; i--)
            {
                if (points[i].Kind != HydrologyPointKind.River)
                    break;

                fadeSteps++;
            }

            if (fadeSteps <= 1)
                return;

            for (int i = 0; i < fadeSteps; i++)
            {
                int index = count - 1 - i;

                float t = i / (float)(fadeSteps - 1);
                float fade = 1f - (t * t * (3f - 2f * t));

                float2Point p = points[index];
                p.Strength *= fade;
                points[index] = p;
            }
        }

        private bool TryFindConfluence(
            float2 pos,
            int ownSegmentId,
            out int hitIndex,
            out bool hitLake)
        {
            hitIndex = -1;
            hitLake = false;

            float bestDistSq = float.MaxValue;

            int count = Points.Length;

            for (int i = 0; i < count; i++)
            {
                float2Point p = Points[i];

                if (p.SegmentId == ownSegmentId)
                    continue;

                float threshold = p.Kind == HydrologyPointKind.Lake
                    ? HydrologySettings.LakeMergeDistance
                    : HydrologySettings.ConfluenceDistance;

                float2 ppos = new float2(p.X, p.Z);
                float distSq = math.distancesq(pos, ppos);

                if (distSq > threshold * threshold)
                    continue;

                if (distSq < bestDistSq)
                {
                    bestDistSq = distSq;
                    hitIndex = i;
                    hitLake = p.Kind == HydrologyPointKind.Lake;
                }
            }

            return hitIndex >= 0;
        }

        private static void BoostDownstream(
            NativeList<float2Point> points,
            int fromIndex,
            int targetSegmentId,
            float bonus)
        {
            for (int i = fromIndex; i < points.Length; i++)
            {
                float2Point p = points[i];

                if (p.SegmentId != targetSegmentId)
                    break;

                if (p.Kind != HydrologyPointKind.River)
                    continue;

                p.Strength += bonus;
                points[i] = p;
            }
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