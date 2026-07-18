using _Project.Features.ProceduralWorld.Infrastructure.Jobs.Settings;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace _Project.Features.ProceduralWorld.Infrastructure.Jobs.Landscape
{
    [BurstCompile]
    public struct LandscapeGenerationJob : IJobParallelFor
    {
        private NativeArray<float> _heights;

        private readonly int _resolution;

        private readonly float _chunkSizeX;
        private readonly float _chunkSizeZ;

        private readonly int2 _chunk;

        private readonly TerrainNoiseSettings _settings;

        [ReadOnly]
        private NativeArray<float2> _octaveOffsets;

        public LandscapeGenerationJob(
            NativeArray<float> heights,
            int resolution,
            float chunkSizeX,
            float chunkSizeZ,
            int2 chunk,
            TerrainNoiseSettings settings,
            NativeArray<float2> octaveOffsets)
        {
            _heights = heights;

            _resolution = resolution;

            _chunkSizeX = chunkSizeX;
            _chunkSizeZ = chunkSizeZ;

            _chunk = chunk;

            _settings = settings;

            _octaveOffsets = octaveOffsets;
        }

        public void Execute(int index)
        {
            int x = index % _resolution;
            int z = index / _resolution;

            float stepX = _chunkSizeX / (_resolution - 1);
            float stepZ = _chunkSizeZ / (_resolution - 1);

            float worldX = _chunk.x * _chunkSizeX + x * stepX + _settings.Offset.x;
            float worldZ = _chunk.y * _chunkSizeZ + z * stepZ + _settings.Offset.y;

            _heights[index] = HeightSampler.Sample(new float2(worldX, worldZ), _settings, _octaveOffsets);
        }
    }
}