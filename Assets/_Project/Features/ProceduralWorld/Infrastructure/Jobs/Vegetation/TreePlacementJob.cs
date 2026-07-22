using _Project.Features.ProceduralWorld.Domain.Landscape;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace _Project.Features.ProceduralWorld.Infrastructure.Jobs.Vegetation
{
    [BurstCompile]
    public struct TreePlacementJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<float> Heights;
        [ReadOnly] public NativeArray<float> RiverMask;
        public int Resolution;
        public float ChunkSizeX;
        public float ChunkSizeZ;
        public int2 ChunkCoord;
        public float CellSize;
        public int CellsX;
        public int CellsZ;
        public float MaxSlopeRad;
        public float MinHeight;
        public float MaxHeight;
        public float DensityNoiseScale;
        public float DensityThreshold;
        public int PrototypeCount;
        public uint WorldSeed;

        public NativeList<TreeInstanceRaw>.ParallelWriter Output;

        public void Execute(int cz)
        {
            float2 chunkOrigin = new float2(
                ChunkCoord.x * ChunkSizeX,
                ChunkCoord.y * ChunkSizeZ);

            for (int cx = 0; cx < CellsX; cx++)
            {
                int worldCellX = (int)math.floor((chunkOrigin.x + cx * CellSize) / CellSize);
                int worldCellZ = (int)math.floor((chunkOrigin.y + cz * CellSize) / CellSize);

                uint seed = Hash(worldCellX, worldCellZ, WorldSeed);
                if (seed == 0) seed = 1;

                var rng = new Unity.Mathematics.Random(seed);

                float jitterX = rng.NextFloat(0f, CellSize);
                float jitterZ = rng.NextFloat(0f, CellSize);

                float localX = cx * CellSize + jitterX;
                float localZ = cz * CellSize + jitterZ;

                if (localX >= ChunkSizeX || localZ >= ChunkSizeZ)
                    continue;

                float u = localX / ChunkSizeX;
                float v = localZ / ChunkSizeZ;

                float height = SampleBilinear(Heights, Resolution, u, v);

                if (height < MinHeight || height > MaxHeight)
                    continue;

                if (RiverMask.Length > 0)
                {
                    float river = SampleBilinear(RiverMask, Resolution, u, v);
                    if (river > 0.01f)
                        continue;
                }

                float slope = EstimateSlope(Heights, Resolution, u, v, ChunkSizeX, ChunkSizeZ);
                if (slope > MaxSlopeRad)
                    continue;

                float density = noise.cnoise(
                    new float2(
                        (chunkOrigin.x + localX) * DensityNoiseScale,
                        (chunkOrigin.y + localZ) * DensityNoiseScale)) * 0.5f + 0.5f;

                if (density < DensityThreshold)
                    continue;

                byte prototype = (byte)rng.NextInt(0, PrototypeCount);

                Output.AddNoResize(new TreeInstanceRaw(
                    new float2(localX, localZ),
                    height,
                    seed,
                    prototype));
            }
        }

        private static uint Hash(int x, int z, uint seed)
        {
            uint h = seed;
            h ^= (uint)x * 0x9E3779B1u;
            h ^= (uint)z * 0x85EBCA77u;
            h ^= h >> 15;
            h *= 0x27D4EB2Fu;
            h ^= h >> 13;
            return h;
        }

        private static float SampleBilinear(NativeArray<float> data, int resolution, float u, float v)
        {
            float fx = u * (resolution - 1);
            float fz = v * (resolution - 1);

            int x0 = (int)fx;
            int z0 = (int)fz;
            int x1 = math.min(x0 + 1, resolution - 1);
            int z1 = math.min(z0 + 1, resolution - 1);

            float tx = fx - x0;
            float tz = fz - z0;

            float h00 = data[z0 * resolution + x0];
            float h10 = data[z0 * resolution + x1];
            float h01 = data[z1 * resolution + x0];
            float h11 = data[z1 * resolution + x1];

            float a = math.lerp(h00, h10, tx);
            float b = math.lerp(h01, h11, tx);

            return math.lerp(a, b, tz);
        }

        private static float EstimateSlope(
            NativeArray<float> heights, int resolution, float u, float v,
            float chunkSizeX, float chunkSizeZ)
        {
            float texelU = 1f / resolution;
            float texelV = 1f / resolution;

            float hL = SampleBilinear(heights, resolution, math.max(u - texelU, 0f), v);
            float hR = SampleBilinear(heights, resolution, math.min(u + texelU, 1f), v);
            float hD = SampleBilinear(heights, resolution, u, math.max(v - texelV, 0f));
            float hU = SampleBilinear(heights, resolution, u, math.min(v + texelV, 1f));

            float dx = (hR - hL) / (2f * texelU * chunkSizeX);
            float dz = (hU - hD) / (2f * texelV * chunkSizeZ);

            return math.atan(math.sqrt(dx * dx + dz * dz));
        }
    }
}