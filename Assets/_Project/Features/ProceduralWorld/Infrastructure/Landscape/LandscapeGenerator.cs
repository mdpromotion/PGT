using _Project.Features.ProceduralWorld.Application.Chunks.Generation;
using _Project.Features.ProceduralWorld.Domain;
using _Project.Features.ProceduralWorld.Domain.Chunks;
using _Project.Features.ProceduralWorld.Domain.Landscape;
using _Project.Features.ProceduralWorld.Infrastructure.Jobs.Landscape;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace _Project.Features.ProceduralWorld.Infrastructure.Landscape
{
    public sealed class LandscapeGenerator :
        IGenerationStage
    {
        private readonly ChunkGrid _grid;

        private readonly TerrainNoiseSettingsProvider _provider;



        public LandscapeGenerator(
            ChunkGrid grid,
            TerrainNoiseSettingsProvider provider)
        {
            _grid = grid;
            _provider = provider;
        }



        public JobHandle Schedule(
            ChunkGenerationState state,
            JobHandle dependency)
        {
            ChunkGenerationContext context =
                state.Context;

            int resolution =
                context.Resolution;

            NativeArray<float> heights =
                new NativeArray<float>(
                    resolution * resolution,
                    Allocator.Persistent);

            LandscapeData landscape =
                new LandscapeData(
                    context.Coordinate,
                    heights,
                    resolution);

            state.Landscape =
                landscape;

            LandscapeGenerationJob job =
                new LandscapeGenerationJob(
                    heights,
                    resolution,
                    _grid.ChunkSizeX,
                    _grid.ChunkSizeZ,
                    new int2(
                        context.Coordinate.X,
                        context.Coordinate.Y),
                    _provider.Create(),
                    _provider.GetOctaveOffsets(
                        _provider.Octaves));

            return job.Schedule(
                heights.Length,
                64,
                dependency);
        }
    }
}