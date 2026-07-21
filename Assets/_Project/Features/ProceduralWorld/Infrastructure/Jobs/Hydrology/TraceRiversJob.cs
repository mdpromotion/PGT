using _Project.Features.ProceduralWorld.Infrastructure.Hydrology;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using _Project.Features.ProceduralWorld.Infrastructure.Jobs.Settings;

namespace _Project.Features.ProceduralWorld.Infrastructure.Jobs.Hydrology
{
    [BurstCompile(FloatMode = FloatMode.Fast, FloatPrecision = FloatPrecision.Low)]
    public struct TraceRiversJob : IJob
    {
        [ReadOnly]
        public NativeArray<int> Sources;

        [ReadOnly]
        public NativeArray<int> FlowTarget;

        [ReadOnly]
        public NativeArray<float> FilledHeights;

        [ReadOnly]
        public NativeArray<float> Accumulation;

        public int Size;
        public float OriginX;
        public float OriginZ;
        public float StepX;
        public float StepZ;

        public HydrologySettings Settings;
        
        public TerrainNoiseSettings NoiseSettings;

        [ReadOnly]
        public NativeArray<float2> NoiseOffsets;

        public NativeList<float2Point> Points;

        public void Execute()
        {
            NativeArray<bool> claimed = new NativeArray<bool>(Size * Size, Allocator.Temp);

            for (int s = 0; s < Sources.Length; s++)
            {
                TraceOne(Sources[s], s, claimed);
            }

            claimed.Dispose();
        }

        private void TraceOne(int startIndex, int segmentId, NativeArray<bool> claimed)
        {
            NativeList<float2Point> river = new NativeList<float2Point>(
                Settings.MaxTraceSteps + 1, Allocator.Temp);

            int index = startIndex;
            int steps = 0;

            while (index >= 0 && steps < Settings.MaxTraceSteps)
            {
                if (claimed[index])
                {
                    AppendPoint(river, index, segmentId, steps);
                    break;
                }

                claimed[index] = true;
                AppendPoint(river, index, segmentId, steps);

                int next = FlowTarget[index];

                if (next < 0)
                    break; 

                index = next;
                steps++;
            }

            ApplyEndFade(river);
            SmoothInto(river, Points);

            river.Dispose();
        }

        private void AppendPoint(NativeList<float2Point> river, int index, int segmentId, int steps)
        {
            int x = index % Size;
            int z = index / Size;

            float2 pos = new float2(
                OriginX + x * StepX,
                OriginZ + z * StepZ);

            float accumulation = Accumulation[index];

            float strength = math.min(
                Settings.MaxRiverStrength,
                math.log(1f + accumulation) / math.max(Settings.AccumulationToStrengthScale, 0.0001f));

            if (steps < Settings.RiverStartFadeSteps)
            {
                float t = steps / (float)Settings.RiverStartFadeSteps;
                strength = math.lerp(Settings.InitialRiverStrength, strength, HydroMath.Smoothstep(t));
            }

            float2 tangent = ResolveTangent(index, x, z);
            float2 perpendicular = new float2(-tangent.y, tangent.x);

            float halfWidth = HydroMath.RiverWidth(
                strength, Settings.RiverWidth, Settings.MaxRiverStrength);

            float2 leftPos = pos - perpendicular * halfWidth;
            float2 rightPos = pos + perpendicular * halfWidth;

            float leftBankHeight = HeightSampler.Sample(leftPos, NoiseSettings, NoiseOffsets);
            float rightBankHeight = HeightSampler.Sample(rightPos, NoiseSettings, NoiseOffsets);

            river.Add(new float2Point
            {
                X = pos.x,
                Z = pos.y,
                Height = FilledHeights[index],
                Strength = strength,
                LeftBankHeight = leftBankHeight,
                RightBankHeight = rightBankHeight,
                SegmentId = segmentId,
                Kind = HydrologyPointKind.River
            });
        }

        private float2 ResolveTangent(int index, int x, int z)
        {
            int next = FlowTarget[index];

            if (next >= 0)
            {
                int nx = next % Size;
                int nz = next / Size;

                float2 dir = new float2((nx - x) * StepX, (nz - z) * StepZ);

                if (math.lengthsq(dir) > 1e-8f)
                    return math.normalize(dir);
            }

            float2 pos = new float2(OriginX + x * StepX, OriginZ + z * StepZ);

            float2 downhill = HeightSampler.SampleDownhillDirection(
                pos, Settings.GradientEpsilon, NoiseSettings, NoiseOffsets, out _);

            return math.lengthsq(downhill) > 1e-8f
                ? math.normalize(downhill)
                : new float2(1f, 0f);
        }

        private static void ApplyEndFade(NativeList<float2Point> points)
        {
            int count = points.Length;

            if (count <= 1)
                return;

            int fadeSteps = math.min(count, 10);

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

        private static void SmoothInto(NativeList<float2Point> river, NativeList<float2Point> output)
        {
            if (river.Length < 3)
            {
                for (int i = 0; i < river.Length; i++)
                    output.Add(river[i]);

                return;
            }

            for (int i = 0; i < river.Length - 1; i++)
            {
                float2Point a = river[i];
                float2Point b = river[i + 1];

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

            output.Add(river[^1]);
        }
    }
}