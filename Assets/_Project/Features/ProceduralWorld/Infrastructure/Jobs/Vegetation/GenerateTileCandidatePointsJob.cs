using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace _Project.Features.ProceduralWorld.Infrastructure.Jobs.Vegetation
{
    [BurstCompile]
    public struct GenerateTileCandidatePointsJob : IJobParallelFor
    {
        public int CellsPerSide;
        public float CellSize;
        
        public float2 TileNoiseOrigin;

        public float JitterStrength;
        public uint TileSeed;

        public float2 PatchNoiseOffset;
        public float PatchScale;
        public int PatchOctaves;
        public float PatchThreshold;

        [WriteOnly] public NativeList<float2>.ParallelWriter Output;

        public void Execute(int index)
        {
            int cellX = index % CellsPerSide;
            int cellZ = index / CellsPerSide;

            uint seed = TileSeed
                        ^ (uint)(cellX * 73856093)
                        ^ (uint)(cellZ * 19349663);

            if (seed == 0)
                seed = 1;

            var rng = new Random(seed);

            // Всегда tile-local.
            float2 cellOrigin = new float2(cellX, cellZ) * CellSize;

            float2 jitter =
                (rng.NextFloat2() - 0.5f)
                * JitterStrength
                * CellSize;

            float2 point =
                cellOrigin
                + new float2(CellSize * 0.5f, CellSize * 0.5f)
                + jitter;

            if (SamplePatchMask(point) < PatchThreshold)
                return;

            Output.AddNoResize(point);
        }

        private float SamplePatchMask(float2 localPosition)
        {
            float2 p =
                (TileNoiseOrigin + localPosition + PatchNoiseOffset)
                / PatchScale;

            float sum = 0f;
            float amplitude = 1f;
            float amplitudeSum = 0f;
            float frequency = 1f;

            for (int i = 0; i < PatchOctaves; i++)
            {
                sum += noise.snoise(p * frequency) * amplitude;
                amplitudeSum += amplitude;

                amplitude *= 0.5f;
                frequency *= 2f;
            }

            float normalized = sum / amplitudeSum;
            return normalized * 0.5f + 0.5f;
        }
    }
}