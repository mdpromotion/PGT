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
        [ReadOnly] public NativeArray<float> WaterSurfaceHeight;

        public int Resolution;

        public float AccumulationThreshold;
        public float FalloffRange;
        public float MaxCarveDepth;

        public float EmbankmentHeight;
        public float EmbankmentPeakPosition;
        public float MinDepthBelowWaterFactor;

        public NativeArray<float> Heights;

        [WriteOnly] public NativeArray<float> RiverMask;

        public void Execute(int index)
        {
            float originalHeight = Heights[index];
            float strength = Accumulation[index];
            float water = WaterSurfaceHeight[index];

            float edgeStart = AccumulationThreshold;
            float range = math.max(FalloffRange, 0.0001f);
            float t = math.saturate((strength - edgeStart) / range);
            
            float carveMask = t * t * (3f - 2f * t);

            float waterStart = 0.12f;
            float waterT = math.saturate((t - waterStart) / (1f - waterStart));
            float waterMask = waterT * waterT * (3f - 2f * waterT);

            RiverMask[index] = waterMask;

            if (carveMask <= 0.0001f)
                return;
            
            float depthMask = carveMask * carveMask;
            float depth = MaxCarveDepth * depthMask;
            float targetBed = originalHeight - depth;
            
            float minBelow = MaxCarveDepth * MinDepthBelowWaterFactor;
            float bedFloor = water - minBelow;
            float constrainedBed = math.min(targetBed, bedFloor);
            targetBed = math.lerp(targetBed, constrainedBed, waterMask);

            float carved = math.lerp(originalHeight, targetBed, carveMask);
            carved = math.min(carved, originalHeight);
            
            float peak = math.clamp(EmbankmentPeakPosition, 0.01f, 0.99f);
            float raw = carveMask < peak
                ? carveMask / peak
                : (1f - carveMask) / (1f - peak);
            raw = math.saturate(raw);
            float embankmentMask = raw * raw * (3f - 2f * raw);

            float finalHeight = carved + (EmbankmentHeight * embankmentMask);

            Heights[index] = math.max(finalHeight, 0f);
        }
    }
}