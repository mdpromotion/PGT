using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace _Project.Features.ProceduralWorld.Infrastructure.Jobs.Vegetation
{
    [BurstCompile]
    public struct FilterVegetationCandidatesJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<float2> TilePoints;

        public float2 ChunkWorldOrigin;
        public float2 ChunkWorldSize;
        public int Resolution;

        public float TerrainHeightWorldScale;
        public float RiverMaskThreshold;
        public uint ChunkSeed;

        [ReadOnly] public NativeArray<float> Heights;
        [ReadOnly] public NativeArray<float> RiverMask;

        [WriteOnly] public NativeList<Domain.Vegetation.VegetationInstanceData>.ParallelWriter Output;

        public void Execute(int index)
        {
            float2 worldPos = TilePoints[index];
            float2 local = worldPos - ChunkWorldOrigin;
            
            if (local.x < 0f || local.y < 0f || local.x >= ChunkWorldSize.x || local.y >= ChunkWorldSize.y)
                return;

            float2 uv = local / ChunkWorldSize;
            float2 grid = uv * (Resolution - 1);

            int x0 = (int)math.floor(grid.x);
            int z0 = (int)math.floor(grid.y);
            int x1 = math.min(x0 + 1, Resolution - 1);
            int z1 = math.min(z0 + 1, Resolution - 1);

            float tx = grid.x - x0;
            float tz = grid.y - z0;

            float h00 = Heights[z0 * Resolution + x0];
            float h10 = Heights[z0 * Resolution + x1];
            float h01 = Heights[z1 * Resolution + x0];
            float h11 = Heights[z1 * Resolution + x1];

            float height01 = math.lerp(math.lerp(h00, h10, tx), math.lerp(h01, h11, tx), tz);

            float r00 = RiverMask[z0 * Resolution + x0];
            float r10 = RiverMask[z0 * Resolution + x1];
            float r01 = RiverMask[z1 * Resolution + x0];
            float r11 = RiverMask[z1 * Resolution + x1];

            float riverMask = math.lerp(math.lerp(r00, r10, tx), math.lerp(r01, r11, tx), tz);

            if (riverMask >= RiverMaskThreshold)
                return;
            
            float cellWorldX = ChunkWorldSize.x / (Resolution - 1);
            float cellWorldZ = ChunkWorldSize.y / (Resolution - 1);

            int cx = math.clamp(x0, 1, Resolution - 2);
            int cz = math.clamp(z0, 1, Resolution - 2);

            float hL = Heights[cz * Resolution + (cx - 1)] * TerrainHeightWorldScale;
            float hR = Heights[cz * Resolution + (cx + 1)] * TerrainHeightWorldScale;
            float hD = Heights[(cz - 1) * Resolution + cx] * TerrainHeightWorldScale;
            float hU = Heights[(cz + 1) * Resolution + cx] * TerrainHeightWorldScale;

            float dHdx = (hR - hL) / (2f * cellWorldX);
            float dHdz = (hU - hD) / (2f * cellWorldZ);

            float slopeRadians = math.atan(math.length(new float2(dHdx, dHdz)));
            float slopeDegrees = math.degrees(slopeRadians);

            float worldHeight = height01 * TerrainHeightWorldScale;
            float3 worldPosition = new float3(worldPos.x, worldHeight, worldPos.y);

            uint seed = ChunkSeed ^ (uint)(index * 2654435761);
            if (seed == 0) seed = 1;

            Output.AddNoResize(new Domain.Vegetation.VegetationInstanceData(
                worldPosition,
                slopeDegrees,
                height01,
                seed));
        }
    }
}