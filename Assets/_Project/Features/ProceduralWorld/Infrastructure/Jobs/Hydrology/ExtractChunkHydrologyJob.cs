using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace _Project.Features.ProceduralWorld.Infrastructure.Jobs.Hydrology
{
    [BurstCompile]
    public struct ExtractChunkHydrologyJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<float> MacroAccumulation;
        [ReadOnly] public NativeArray<sbyte> MacroFlowDirection;
        public int MacroResolution;
        public float2 MacroCellSize;
        public float2 MacroOriginWorld;

        public float2 ChunkOriginWorld;
        public float2 ChunkWorldSize;
        public int ChunkResolution;

        [WriteOnly] public NativeArray<float> OutAccumulation;
        [WriteOnly] public NativeArray<sbyte> OutFlowDirection;

        public void Execute(int index)
        {
            int x = index % ChunkResolution;
            int y = index / ChunkResolution;

            float2 t = ChunkResolution > 1
                ? new float2(x, y) / (ChunkResolution - 1)
                : float2.zero;

            float2 worldPos = ChunkOriginWorld + t * ChunkWorldSize;
            float2 macroLocal = (worldPos - MacroOriginWorld) / MacroCellSize;

            OutAccumulation[index] = SampleBilinear(macroLocal);
            OutFlowDirection[index] = SampleNearestDirection(macroLocal);
        }

        private float SampleBilinear(float2 macroLocal)
        {
            int maxIndex = MacroResolution - 1;

            float fx = math.clamp(macroLocal.x, 0f, maxIndex);
            float fy = math.clamp(macroLocal.y, 0f, maxIndex);

            int x0 = (int)math.floor(fx);
            int y0 = (int)math.floor(fy);
            int x1 = math.min(x0 + 1, maxIndex);
            int y1 = math.min(y0 + 1, maxIndex);

            float tx = fx - x0;
            float ty = fy - y0;

            float h00 = MacroAccumulation[y0 * MacroResolution + x0];
            float h10 = MacroAccumulation[y0 * MacroResolution + x1];
            float h01 = MacroAccumulation[y1 * MacroResolution + x0];
            float h11 = MacroAccumulation[y1 * MacroResolution + x1];

            float top = math.lerp(h00, h10, tx);
            float bottom = math.lerp(h01, h11, tx);
            return math.lerp(top, bottom, ty);
        }

        private sbyte SampleNearestDirection(float2 macroLocal)
        {
            int maxIndex = MacroResolution - 1;
            int x = math.clamp((int)math.round(macroLocal.x), 0, maxIndex);
            int y = math.clamp((int)math.round(macroLocal.y), 0, maxIndex);
            return MacroFlowDirection[y * MacroResolution + x];
        }
    }
}