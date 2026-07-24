using _Project.Features.ProceduralWorld.Application.Chunks.Generation;
using _Project.Features.ProceduralWorld.Domain;
using _Project.Features.ProceduralWorld.Domain.Chunks;
using _Project.Features.ProceduralWorld.Domain.Hydrology;
using _Project.Features.ProceduralWorld.Domain.World;
using _Project.Features.ProceduralWorld.Infrastructure.Jobs.Hydrology;
using _Project.Features.ProceduralWorld.Infrastructure.Jobs.Landscape;
using _Project.Features.ProceduralWorld.Infrastructure.Landscape;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace _Project.Features.ProceduralWorld.Infrastructure.Hydrology
{
    public sealed class HydrologyGenerator : IGenerationStage
    {
        private readonly MacroZoneGrid _macroGrid;
        private readonly HydrologySettings _settings;
        private readonly TerrainNoiseSettingsProvider _noiseProvider;
        private readonly MacroZoneHydrologyCache _cache;

        public HydrologyGenerator(
            MacroZoneGrid macroGrid,
            HydrologySettings settings,
            TerrainNoiseSettingsProvider noiseProvider,
            MacroZoneHydrologyCache cache)
        {
            _macroGrid = macroGrid;
            _settings = settings;
            _noiseProvider = noiseProvider;
            _cache = cache;
        }

        public JobHandle Schedule(
            ChunkGenerationState state,
            JobHandle dependency)
        {
            Debug.Log(123);
            
            ChunkGenerationContext context = state.Context;
            MacroZoneCoordinate zone = _macroGrid.ToMacroZoneCoordinate(context.Coordinate);

            MacroZoneHandle macroZone = _cache.Acquire(zone, _macroGrid, _settings, _noiseProvider);

            int resolution = context.Resolution;
            int cellCount = resolution * resolution;

            NativeArray<float> accumulationOut = new NativeArray<float>(cellCount, Allocator.Persistent);
            NativeArray<sbyte> flowDirectionOut = new NativeArray<sbyte>(cellCount, Allocator.Persistent);

            float2 chunkOriginWorld = ComputeChunkWorldOrigin(context, zone);
            float2 chunkWorldSize = new float2(
                macroZone.CellSize.x * _settings.CellsPerChunk,
                macroZone.CellSize.y * _settings.CellsPerChunk);

            var extractJob = new ExtractChunkHydrologyJob
            {
                MacroAccumulation = macroZone.Accumulation,
                MacroFlowDirection = macroZone.FlowDirection,
                MacroResolution = macroZone.Resolution,
                MacroCellSize = macroZone.CellSize,
                MacroOriginWorld = macroZone.OriginWorld,
                ChunkOriginWorld = chunkOriginWorld,
                ChunkWorldSize = chunkWorldSize,
                ChunkResolution = resolution,
                OutAccumulation = accumulationOut,
                OutFlowDirection = flowDirectionOut,
            };

            JobHandle combinedDependency = JobHandle.CombineDependencies(dependency, macroZone.GenerationHandle);
            JobHandle handle = extractJob.Schedule(cellCount, 64, combinedDependency);

            state.Hydrology = new HydrologyData(
                context.Coordinate,
                accumulationOut,
                flowDirectionOut,
                resolution,
                onDispose: () => _cache.Release(zone));

            return handle;
        }

        private float2 ComputeChunkWorldOrigin(ChunkGenerationContext context, MacroZoneCoordinate zone)
        {
            UnityEngine.Vector2Int local = _macroGrid.ToLocalChunkOffset(context.Coordinate);
            UnityEngine.Vector2 zoneOrigin = _macroGrid.ToZoneWorldOrigin(zone);

            float chunkSizeX = _macroGrid.ZoneWorldSizeX / _macroGrid.ZoneSizeInChunks;
            float chunkSizeZ = _macroGrid.ZoneWorldSizeZ / _macroGrid.ZoneSizeInChunks;

            return new float2(
                zoneOrigin.x + local.x * chunkSizeX,
                zoneOrigin.y + local.y * chunkSizeZ);
        }
    }
}