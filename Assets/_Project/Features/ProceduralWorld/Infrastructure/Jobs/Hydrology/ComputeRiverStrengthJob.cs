using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace _Project.Features.ProceduralWorld.Infrastructure.Jobs.Hydrology
{
    [BurstCompile]
    public struct ComputeRiverStrengthJob : IJobParallelFor
    {
        public int Resolution;
        public float2 ChunkWorldOrigin;
        public float2 ChunkWorldSize;

        public int MacroPaddedSize;
        public int MacroPaddingCells;
        public int MacroTileCells;
        public int MacroRiverZoneMargin;
        public float MacroCellSize;
        public float2 MacroWorldOrigin;

        [ReadOnly] public NativeArray<float> MacroAccumulation;
        [ReadOnly] public NativeArray<float> MacroHeights;
        [ReadOnly] public NativeArray<float> MacroWaterLevels;
        public float LocalAccumulationNormalizationRange;

        [WriteOnly] public NativeArray<float> RiverStrength;
        [WriteOnly] public NativeArray<float> MacroHeightSample;
        [WriteOnly] public NativeArray<float> WaterSurfaceHeight;

        public void Execute(int index)
        {
            int x = index % Resolution;
            int z = index / Resolution;

            float u = Resolution > 1 ? (float)x / (Resolution - 1) : 0f;
            float v = Resolution > 1 ? (float)z / (Resolution - 1) : 0f;

            float2 worldPos = ChunkWorldOrigin + new float2(u, v) * ChunkWorldSize;
            float2 macroCoord = (worldPos - MacroWorldOrigin) / MacroCellSize;

            float localAccum = SampleBilinear(MacroAccumulation, macroCoord);
            
            MacroHeightSample[index] = SampleBilinear(MacroHeights, macroCoord);
            
            WaterSurfaceHeight[index] = SampleBilinear(MacroWaterLevels, macroCoord);

            float strength = math.saturate(
                (localAccum - 1f) / math.max(LocalAccumulationNormalizationRange, 0.0001f));

            strength *= EdgeFade(macroCoord.x, macroCoord.y);

            RiverStrength[index] = strength;
        }

        private float EdgeFade(float gx, float gz)
        {
            if (MacroRiverZoneMargin <= 0)
                return 1f;

            float coreMin = MacroPaddingCells;
            float coreMax = MacroPaddingCells + MacroTileCells;

            float distX = math.min(gx - coreMin, coreMax - 1f - gx);
            float distZ = math.min(gz - coreMin, coreMax - 1f - gz);
            float dist = math.min(distX, distZ);

            return math.smoothstep(0f, MacroRiverZoneMargin, dist);
        }
        
        private float SampleBilinear(NativeArray<float> field, float2 macroCoord)
        {
            float gx = math.clamp(macroCoord.x, 0f, MacroPaddedSize - 1.0001f);
            float gz = math.clamp(macroCoord.y, 0f, MacroPaddedSize - 1.0001f);

            int x0 = (int)gx;
            int z0 = (int)gz;
            int x1 = x0 + 1;
            int z1 = z0 + 1;

            float tx = gx - x0;
            float tz = gz - z0;

            float h00 = field[z0 * MacroPaddedSize + x0];
            float h10 = field[z0 * MacroPaddedSize + x1];
            float h01 = field[z1 * MacroPaddedSize + x0];
            float h11 = field[z1 * MacroPaddedSize + x1];

            float a = math.lerp(h00, h10, tx);
            float b = math.lerp(h01, h11, tx);

            return math.lerp(a, b, tz);
        }
    }
}