using _Project.Features.ProceduralWorld.Application.Chunks.Generation;
using _Project.Features.ProceduralWorld.Domain;
using _Project.Features.ProceduralWorld.Domain.Chunks;
using _Project.Features.ProceduralWorld.Domain.Vegetation;
using _Project.Features.ProceduralWorld.Domain.World;
using _Project.Features.ProceduralWorld.Infrastructure.Jobs.Vegetation;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace _Project.Features.ProceduralWorld.Infrastructure.Vegetation
{
    public sealed class VegetationGenerator : IGenerationStage
    {
        private readonly ChunkGrid _chunkGrid;
        private readonly VegetationTileCache _tileCache;
        private readonly VegetationGridSettings _settings;
        private readonly float _terrainHeightWorldScale;
        private readonly int _worldSeed;

        public VegetationGenerator(
            ChunkGrid chunkGrid,
            VegetationTileCache tileCache,
            VegetationGridSettings settings,
            WorldSettings worldSettings,
            float terrainHeightWorldScale)
        {
            _chunkGrid = chunkGrid;
            _tileCache = tileCache;
            _settings = settings;
            _worldSeed = worldSettings.Seed;
            _terrainHeightWorldScale = terrainHeightWorldScale;
        }

        public JobHandle Schedule(
            ChunkGenerationState state,
            JobHandle dependency)
        {
            ChunkCoordinate coordinate =
                state.Context.Coordinate;

            int resolution =
                state.Context.Resolution;

            VegetationTileCoordinate tileCoordinate =
                _tileCache.ToTileCoordinate(coordinate);

            VegetationTileData tile =
                _tileCache.GetOrBuild(tileCoordinate);

            // Generation-space.
            // Большое умножение coordinate × chunkSize
            // выполняется в double.
            double2 absoluteChunkOrigin =
                GenerationSpace.AbsoluteChunkOrigin(
                    coordinate,
                    _chunkGrid.ChunkSizeX,
                    _chunkGrid.ChunkSizeZ);

            // Origin того же tile, в котором лежат TilePoints.
            double2 tileOrigin =
                _tileCache.GetTileWorldOrigin(tileCoordinate);

            // Маленькая величина внутри tile.
            float2 chunkOrigin =
                GenerationSpace.LocalOffset(
                    absoluteChunkOrigin,
                    tileOrigin);

            float2 chunkSize =
                new float2(
                    _chunkGrid.ChunkSizeX,
                    _chunkGrid.ChunkSizeZ);

            var instances = new NativeList<VegetationInstanceData>(
                tile.Points.Length,
                Allocator.Persistent);

            state.Vegetation =
                new VegetationData(
                    coordinate,
                    instances);

            uint chunkSeed =
                (uint)_worldSeed
                ^ (uint)(coordinate.X * 374761393)
                ^ (uint)(coordinate.Y * 668265263);

            var job = new FilterVegetationCandidatesJob
            {
                TilePoints =
                    tile.Points.AsDeferredJobArray(),
                
                ChunkWorldOrigin = chunkOrigin,

                ChunkWorldSize = chunkSize,
                Resolution = resolution,

                TerrainHeightWorldScale =
                    _terrainHeightWorldScale,

                RiverMaskThreshold =
                    _settings.RiverMaskThreshold,

                ChunkSeed = chunkSeed,

                Heights =
                    state.Landscape.Heights,

                RiverMask =
                    state.Hydrology.RiverMask,

                Output =
                    instances.AsParallelWriter(),
            };

            return job.Schedule(
                tile.Points.Length,
                64,
                dependency);
        }
    }
}