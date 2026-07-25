using _Project.Features.ProceduralWorld.Application.Chunks.Generation;
using _Project.Features.ProceduralWorld.Domain;
using _Project.Features.ProceduralWorld.Domain.Chunks;
using _Project.Features.ProceduralWorld.Domain.Hydrology;
using _Project.Features.ProceduralWorld.Domain.World;
using _Project.Features.ProceduralWorld.Infrastructure.Jobs.Hydrology;
using Unity.Jobs;
using Unity.Mathematics;

namespace _Project.Features.ProceduralWorld.Infrastructure.Hydrology
{
    public sealed class HydrologyGenerator : IGenerationStage
    {
        private readonly ChunkGrid _chunkGrid;
        private readonly MacroRegionCache _macroRegionCache;
        private readonly float _localAccumulationNormalizationRange;

        public HydrologyGenerator(
            ChunkGrid chunkGrid,
            MacroRegionCache macroRegionCache,
            float localAccumulationNormalizationRange)
        {
            _chunkGrid = chunkGrid;
            _macroRegionCache = macroRegionCache;
            _localAccumulationNormalizationRange = localAccumulationNormalizationRange;
        }

        public JobHandle Schedule(
            ChunkGenerationState state,
            JobHandle dependency)
        {
            int resolution = state.Context.Resolution;

            state.Hydrology = new HydrologyData(
                state.Context.Coordinate,
                resolution,
                onDispose: null);

            float2 chunkOrigin = GetChunkWorldOrigin(state.Context.Coordinate);
            float2 chunkSize = new float2(_chunkGrid.ChunkSizeX, _chunkGrid.ChunkSizeZ);

            MacroRegionCoordinate regionCoordinate = _macroRegionCache.ToRegionCoordinate(chunkOrigin);
            MacroRegionData region = _macroRegionCache.GetOrBuild(regionCoordinate);

            var job = new ComputeRiverStrengthJob
            {
                Resolution = resolution,
                ChunkWorldOrigin = chunkOrigin,
                ChunkWorldSize = chunkSize,

                MacroPaddedSize = region.PaddedSize,
                MacroCellSize = region.CellSize,
                MacroWorldOrigin = region.WorldOrigin,
                MacroAccumulation = region.Accumulation,
                LocalAccumulationNormalizationRange = _localAccumulationNormalizationRange,

                RiverStrength = state.Hydrology.Accumulation,
            };

            return job.Schedule(resolution * resolution, 64, dependency);
        }

        private float2 GetChunkWorldOrigin(ChunkCoordinate coordinate)
        {
            var offset = _chunkGrid.ToWorldOffset(coordinate);
            return new float2(offset.x, offset.y);
        }
    }
}