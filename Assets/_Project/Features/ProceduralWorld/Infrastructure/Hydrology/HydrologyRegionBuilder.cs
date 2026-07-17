using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using _Project.Features.ProceduralWorld.Domain;
using _Project.Features.ProceduralWorld.Domain.Chunks;
using _Project.Features.ProceduralWorld.Domain.World;
using _Project.Features.ProceduralWorld.Infrastructure.Jobs;
using _Project.Features.ProceduralWorld.Infrastructure.Jobs.Settings;
using _Project.Features.ProceduralWorld.Infrastructure.Landscape;
using UnityEditor.Rendering;
using UnityEngine;

namespace _Project.Features.ProceduralWorld.Infrastructure.Hydrology
{
    public sealed class HydrologyRegionBuilder
    {
        private readonly ChunkGrid _grid;
        private readonly WorldSettings _worldSettings;
        private readonly HydrologySettings _hydrologySettings;
        private readonly TerrainNoiseSettingsProvider _terrainSettingsProvider;

        public HydrologyRegionBuilder(
            ChunkGrid grid,
            WorldSettings worldSettings,
            HydrologySettings hydrologySettings,
            TerrainNoiseSettingsProvider terrainSettingsProvider)
        {
            _grid = grid;
            _worldSettings = worldSettings;
            _hydrologySettings = hydrologySettings;
            _terrainSettingsProvider = terrainSettingsProvider;
        }

        public RegionFetchResult Schedule(RegionCoordinate region)
        {
            int size = _hydrologySettings.RegionCoarseResolution;

            float regionWorldSizeX =
                _grid.ChunkSizeX * _hydrologySettings.RegionSizeInChunks;

            float regionWorldSizeZ =
                _grid.ChunkSizeZ * _hydrologySettings.RegionSizeInChunks;

            float originX = region.X * regionWorldSizeX;
            float originZ = region.Y * regionWorldSizeZ;

            float stepX = regionWorldSizeX / (size - 1);
            float stepZ = regionWorldSizeZ / (size - 1);

            TerrainNoiseSettings settings = _terrainSettingsProvider.Create();

            NativeArray<float2> offsets =
                _terrainSettingsProvider.GetOctaveOffsets(_worldSettings.Octaves);
            
            NativeArray<float> heights =
                new NativeArray<float>(size * size, Allocator.Persistent);

            HydrologyHeightFieldJob heightJob = new HydrologyHeightFieldJob
            {
                Heights = heights,
                Size = size,
                OriginX = originX,
                OriginZ = originZ,
                StepX = stepX,
                StepZ = stepZ,
                Settings = settings,
                Offsets = offsets
            };

            JobHandle heightHandle = heightJob.Schedule(heights.Length, 64);

            int initialCapacity = math.max(
                1,
                _hydrologySettings.RiverSourceCount *
                (_hydrologySettings.MaxTraceSteps + 1) * 2);
            
            NativeList<float2Point> points =
                new NativeList<float2Point>(initialCapacity, Allocator.Persistent);
            
            HydrologyRiverTraceJob traceJob = new HydrologyRiverTraceJob
            {
                Heights = heights,
                Size = size,
                OriginX = originX,
                OriginZ = originZ,
                StepX = stepX,
                StepZ = stepZ,
                Settings = settings,
                Offsets = offsets,
                HydrologySettings = _hydrologySettings,
                WorldSeed = _worldSettings.Seed,
                RegionCoord = new int2(region.X, region.Y),
                Points = points
            };

            JobHandle traceHandle = traceJob.Schedule(heightHandle);
            
            JobHandle disposeHandle = heights.Dispose(traceHandle);

            return new RegionFetchResult(
                new HydrologyRegionData(points),
                disposeHandle);
        }
    }
}