using _Project.Features.ProceduralWorld.Application.Chunks.Generation;
using _Project.Features.ProceduralWorld.Domain;
using _Project.Features.ProceduralWorld.Domain.Chunks;
using _Project.Features.ProceduralWorld.Domain.Landscape;
using _Project.Features.ProceduralWorld.Infrastructure;
using _Project.Features.ProceduralWorld.Infrastructure.Jobs.Vegetation;
using _Project.Features.ProceduralWorld.Infrastructure.Vegetation;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

public sealed class TreeGenerator : IGenerationStage
{
    private readonly ChunkGrid _grid;
    private readonly TreePlacementSettings _settings;

    public TreeGenerator(ChunkGrid grid, TreePlacementSettings settings)
    {
        _grid = grid;
        _settings = settings;
    }

    public JobHandle Schedule(ChunkGenerationState state, JobHandle dependency)
    {
        ChunkGenerationContext context = state.Context;

        int cellsX = (int)math.ceil(_grid.ChunkSizeX / _settings.CellSize);
        int cellsZ = (int)math.ceil(_grid.ChunkSizeZ / _settings.CellSize);

        NativeList<TreeInstanceRaw> output =
            new NativeList<TreeInstanceRaw>(cellsX * cellsZ, Allocator.Persistent);

        NativeArray<float> riverMask = state.Landscape.RiverMask;

        TreePlacementJob job = new TreePlacementJob
        {
            Heights = state.Landscape.Heights,
            RiverMask = riverMask,
            Resolution = context.Resolution,
            ChunkSizeX = _grid.ChunkSizeX,
            ChunkSizeZ = _grid.ChunkSizeZ,
            ChunkCoord = new int2(context.Coordinate.X, context.Coordinate.Y),
            CellSize = _settings.CellSize,
            CellsX = cellsX,
            CellsZ = cellsZ,
            MaxSlopeRad = math.radians(_settings.MaxSlopeDegrees),
            MinHeight = _settings.MinHeight,
            MaxHeight = _settings.MaxHeight,
            DensityNoiseScale = _settings.DensityNoiseScale,
            DensityThreshold = _settings.DensityThreshold,
            PrototypeCount = _settings.PrototypeCount,
            WorldSeed = _settings.WorldSeed,
            Output = output.AsParallelWriter()
        };

        state.Landscape.AttachTrees(output);
        
        return job.Schedule(cellsZ, 1, dependency);
    }
}