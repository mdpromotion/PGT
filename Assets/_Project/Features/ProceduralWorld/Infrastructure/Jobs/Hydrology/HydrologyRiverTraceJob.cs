using _Project.Features.ProceduralWorld.Infrastructure.Hydrology;
using _Project.Features.ProceduralWorld.Infrastructure.Jobs.Settings;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

namespace _Project.Features.ProceduralWorld.Infrastructure.Jobs.Hydrology
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
        public int WorldSeed;

        [ReadOnly]
        public NativeArray<float2> Offsets;

        public HydrologySettings HydrologySettings;

        public int2 RegionCoord;

        public NativeList<float2Point> Points;

        public void Execute()
        {
            int seed =
                (int)math.hash(RegionCoord) ^
                WorldSeed;

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

        private int PickHighPoint(ref Random random, float averageHeight)
        {
            int bestId = -1;
            float bestHeight = averageHeight;

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

            return bestId >= 0 ? bestId : 0;
        }

        private float2 ResolveBankSamplingTangent(float2 pos, float2 direction)
        {
            if (math.lengthsq(direction) > 1e-8f)
                return math.normalize(direction);

            float2 downhill = HeightSampler.SampleDownhillDirection(
                pos,
                HydrologySettings.GradientEpsilon,
                Settings,
                Offsets,
                out _);

            return math.lengthsq(downhill) > 1e-8f
                ? math.normalize(downhill)
                : new float2(1f, 0f);
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

            float2 direction = float2.zero;
            float strength = HydrologySettings.InitialRiverStrength;
            float previousHeight = height;

            float meanderPhase = 0f;
            float meanderTarget = random.NextFloat(-1f, 1f);

            int stepsTaken = 0;
            bool endedByConfluence = false;

            for (int i = 0; i < HydrologySettings.MaxTraceSteps; i++)
            {
                float terrain = HeightSampler.Sample(pos, Settings, Offsets);

                float2 tangent = ResolveBankSamplingTangent(pos, direction);
                float2 perpendicular = new float2(-tangent.y, tangent.x);

                float halfWidth = HydroMath.RiverWidth(
                    strength, HydrologySettings.RiverWidth, HydrologySettings.MaxRiverStrength);

                float2 leftPos = pos - perpendicular * halfWidth;
                float2 rightPos = pos + perpendicular * halfWidth;

                float leftBankHeight = HeightSampler.Sample(leftPos, Settings, Offsets);
                float rightBankHeight = HeightSampler.Sample(rightPos, Settings, Offsets);

                points.Add(new float2Point
                {
                    X = pos.x,
                    Z = pos.y,
                    Height = math.min(terrain, previousHeight),
                    Strength = strength,
                    LeftBankHeight = leftBankHeight,
                    RightBankHeight = rightBankHeight,
                    SegmentId = segmentId,
                    Kind = HydrologyPointKind.River
                });

                stepsTaken++;

                if (TryFindConfluence(pos, segmentId, out int hitIndex))
                {
                    float2Point hitPoint = Points[hitIndex];

                    float2Point last = points[points.Length - 1];
                    last.X = hitPoint.X;
                    last.Z = hitPoint.Z;
                    points[points.Length - 1] = last;

                    BlendApproachToConfluence(
                        points,
                        hitPoint.Strength,
                        hitPoint.Height,
                        HydrologySettings.RiverStartFadeSteps);

                    float boostedStrength = math.min(
                        math.max(hitPoint.Strength, strength * HydrologySettings.ConfluenceStrengthFactor),
                        HydrologySettings.MaxRiverStrength);

                    BoostDownstream(
                        Points,
                        hitIndex,
                        hitPoint.SegmentId,
                        boostedStrength,
                        HydrologySettings.RiverStartFadeSteps);

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

                meanderPhase = math.lerp(
                    meanderPhase,
                    meanderTarget,
                    HydrologySettings.MeanderResponsiveness);

                if (math.abs(meanderPhase - meanderTarget) < 0.05f)
                {
                    meanderTarget = random.NextFloat(-1f, 1f);
                }

                float slopeStraighten = math.saturate(slope * HydrologySettings.SlopeStraightenFactor);
                float meanderScale = 1f - slopeStraighten;

                float wobble = meanderPhase * HydrologySettings.MeanderStrength * meanderScale;

                float2 wobbled = math.normalize(
                    desired +
                    new float2(-desired.y, desired.x) * wobble);

                float2 newDirection =
                    math.lengthsq(direction) > 1e-8f
                        ? ClampTurn(direction, wobbled, HydrologySettings.MaxTurnAnglePerStep)
                        : wobbled;

                direction = newDirection;

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

                    strength = math.lerp(
                        HydrologySettings.InitialRiverStrength,
                        targetStrength,
                        HydroMath.Smoothstep(t));
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

        private static float2 ClampTurn(float2 from, float2 to, float maxAngleRadians)
        {
            float angleFrom = math.atan2(from.y, from.x);
            float angleTo = math.atan2(to.y, to.x);

            float delta = angleTo - angleFrom;

            delta = math.atan2(math.sin(delta), math.cos(delta));

            float clampedDelta = math.clamp(delta, -maxAngleRadians, maxAngleRadians);

            float finalAngle = angleFrom + clampedDelta;

            return new float2(math.cos(finalAngle), math.sin(finalAngle));
        }

        private static void BlendApproachToConfluence(
            NativeList<float2Point> points,
            float targetStrength,
            float targetHeight,
            int fadeSteps)
        {
            int count = points.Length;
            int steps = math.min(math.max(fadeSteps, 1), count);

            for (int i = 0; i < steps; i++)
            {
                int index = count - 1 - i;

                float t = steps > 1 ? 1f - i / (float)(steps - 1) : 1f;
                float smooth = HydroMath.Smoothstep(t);

                float2Point p = points[index];
                p.Strength = math.lerp(p.Strength, targetStrength, smooth);
                p.Height = math.lerp(p.Height, targetHeight, smooth);
                points[index] = p;
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
                float fade = 1f - HydroMath.Smoothstep(t);

                float2Point p = points[index];
                p.Strength *= fade;
                points[index] = p;
            }
        }

        private bool TryFindConfluence(
            float2 pos,
            int ownSegmentId,
            out int hitIndex)
        {
            hitIndex = -1;

            float bestDistSq = float.MaxValue;
            float thresholdSq = HydrologySettings.ConfluenceDistance * HydrologySettings.ConfluenceDistance;

            int count = Points.Length;

            for (int i = 0; i < count; i++)
            {
                float2Point p = Points[i];

                if (p.SegmentId == ownSegmentId)
                    continue;

                float2 ppos = new float2(p.X, p.Z);
                float distSq = math.distancesq(pos, ppos);

                if (distSq > thresholdSq)
                    continue;

                if (distSq < bestDistSq)
                {
                    bestDistSq = distSq;
                    hitIndex = i;
                }
            }

            return hitIndex >= 0;
        }

        private static void BoostDownstream(
            NativeList<float2Point> points,
            int fromIndex,
            int targetSegmentId,
            float targetStrength,
            int fadeSteps)
        {
            int steps = 0;

            for (int i = fromIndex; i < points.Length; i++)
            {
                float2Point p = points[i];

                if (p.SegmentId != targetSegmentId)
                    break;

                if (p.Kind != HydrologyPointKind.River)
                    continue;

                float t = fadeSteps > 0 ? math.saturate(steps / (float)fadeSteps) : 1f;
                float smooth = HydroMath.Smoothstep(t);

                float blended = math.lerp(p.Strength, targetStrength, smooth);
                p.Strength = math.min(math.max(p.Strength, blended), targetStrength);

                points[i] = p;
                steps++;
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

                output.Add(new float2Point
                {
                    X = math.lerp(a.X, b.X, .5f),
                    Z = math.lerp(a.Z, b.Z, .5f),
                    Height = math.lerp(a.Height, b.Height, .5f),
                    Strength = math.lerp(a.Strength, b.Strength, .5f),
                    LeftBankHeight = math.lerp(a.LeftBankHeight, b.LeftBankHeight, .5f),
                    RightBankHeight = math.lerp(a.RightBankHeight, b.RightBankHeight, .5f),
                    SegmentId = a.SegmentId,
                    Kind = HydrologyPointKind.River
                });
            }

            output.Add(riverPoints[^1]);
        }
    }
}