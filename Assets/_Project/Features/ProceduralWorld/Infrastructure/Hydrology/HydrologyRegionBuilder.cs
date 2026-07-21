using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using _Project.Features.ProceduralWorld.Domain;
using _Project.Features.ProceduralWorld.Domain.Chunks;
using _Project.Features.ProceduralWorld.Domain.World;
using _Project.Features.ProceduralWorld.Infrastructure.Jobs.Hydrology;
using _Project.Features.ProceduralWorld.Infrastructure.Jobs.Settings;
using _Project.Features.ProceduralWorld.Infrastructure.Landscape;

namespace _Project.Features.ProceduralWorld.Infrastructure.Hydrology
{
    public sealed class HydrologyRegionBuilder : System.IDisposable
    {
        private readonly ChunkGrid _grid;
        private readonly WorldSettings _worldSettings;
        private readonly HydrologySettings _hydrologySettings;
        private readonly TerrainNoiseSettingsProvider _terrainSettingsProvider;

        private readonly NativeArray<int2> _neighbors8;

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

            _neighbors8 = new NativeArray<int2>(8, Allocator.Persistent)
            {
                [0] = new int2(-1, -1), [1] = new int2(0, -1), [2] = new int2(1, -1),
                [3] = new int2(-1,  0),                        [4] = new int2(1,  0),
                [5] = new int2(-1,  1), [6] = new int2(0,  1), [7] = new int2(1,  1)
            };
        }

        public RegionFetchResult Schedule(RegionCoordinate region)
        {
            int core = _hydrologySettings.RegionCoarseResolution;
            int margin = _hydrologySettings.RegionMarginCells;
            int size = core + margin * 2;
            int cellCount = size * size;

            float regionWorldSizeX = _grid.ChunkSizeX * _hydrologySettings.RegionSizeInChunks;
            float regionWorldSizeZ = _grid.ChunkSizeZ * _hydrologySettings.RegionSizeInChunks;
            
            float stepX = regionWorldSizeX / (core - 1);
            float stepZ = regionWorldSizeZ / (core - 1);
            
            float originX = region.X * regionWorldSizeX - margin * stepX;
            float originZ = region.Y * regionWorldSizeZ - margin * stepZ;

            TerrainNoiseSettings noiseSettings = _terrainSettingsProvider.Create();
            NativeArray<float2> offsets = _terrainSettingsProvider.GetOctaveOffsets(_worldSettings.Octaves);
            
            NativeArray<float> rawHeights = new NativeArray<float>(cellCount, Allocator.Persistent);

            JobHandle heightHandle = new HydrologyHeightFieldJob
            {
                Heights = rawHeights,
                Size = size,
                OriginX = originX,
                OriginZ = originZ,
                StepX = stepX,
                StepZ = stepZ,
                Settings = noiseSettings,
                Offsets = offsets
            }.Schedule(cellCount, 64);
            
            NativeArray<float> filledHeights = new NativeArray<float>(cellCount, Allocator.Persistent);

            JobHandle fillHandle = new DepressionFillJob
            {
                RawHeights = rawHeights,
                Neighbors8 = _neighbors8,
                Size = size,
                FilledHeights = filledHeights
            }.Schedule(heightHandle);
            
            NativeArray<int> flowTarget = new NativeArray<int>(cellCount, Allocator.Persistent);

            JobHandle flowDirHandle = new FlowDirectionJob
            {
                FilledHeights = filledHeights,
                Neighbors8 = _neighbors8,
                Size = size,
                StepX = stepX,
                StepZ = stepZ,
                FlowTarget = flowTarget
            }.Schedule(cellCount, 64, fillHandle);
            
            NativeArray<HeightIndex> order = new NativeArray<HeightIndex>(cellCount, Allocator.Persistent);

            JobHandle orderBuildHandle = new BuildFlowOrderJob
            {
                FilledHeights = filledHeights,
                Order = order
            }.Schedule(cellCount, 128, fillHandle);

            JobHandle orderSortHandle = new SortFlowOrderJob
            {
                Order = order
            }.Schedule(orderBuildHandle);
            
            NativeArray<float> accumulation = new NativeArray<float>(cellCount, Allocator.Persistent);

            JobHandle initAccumHandle = new FillFloatJob
            {
                Values = accumulation,
                Value = 1f
            }.Schedule(cellCount, 256);

            JobHandle accumDepsHandle = JobHandle.CombineDependencies(
                flowDirHandle, orderSortHandle, initAccumHandle);

            JobHandle accumHandle = new FlowAccumulationJob
            {
                Order = order,
                FlowTarget = flowTarget,
                Accumulation = accumulation
            }.Schedule(accumDepsHandle);
            
            NativeList<int> sources = new NativeList<int>(cellCount / 8, Allocator.Persistent);

            JobHandle sourcesHandle = new RiverSourceExtractionJob
            {
                Accumulation = accumulation,
                FlowTarget = flowTarget,
                Neighbors8 = _neighbors8,
                Size = size,
                Threshold = _hydrologySettings.FlowAccumulationThreshold,
                Sources = sources.AsParallelWriter()
            }.Schedule(cellCount, 64, accumHandle);
            
            int initialPointsCapacity = math.max(1, _hydrologySettings.MaxTraceSteps * 32);
            NativeList<float2Point> points = new NativeList<float2Point>(initialPointsCapacity, Allocator.Persistent);

            JobHandle traceHandle = new TraceRiversJob
            {
                Sources = sources.AsDeferredJobArray(),
                FlowTarget = flowTarget,
                FilledHeights = filledHeights,
                Accumulation = accumulation,
                Size = size,
                OriginX = originX,
                OriginZ = originZ,
                StepX = stepX,
                StepZ = stepZ,
                Settings = _hydrologySettings,
                NoiseSettings = noiseSettings,
                NoiseOffsets = offsets,
                Points = points
            }.Schedule(sourcesHandle);
            
            TrimPointsToRegionJob trimJob = new TrimPointsToRegionJob
            {
                RegionOriginX = region.X * regionWorldSizeX,
                RegionOriginZ = region.Y * regionWorldSizeZ,
                RegionSizeX = regionWorldSizeX,
                RegionSizeZ = regionWorldSizeZ,
                CarveMargin = math.max(_hydrologySettings.RiverWidth * 3f, 0.001f),
                Points = points.AsDeferredJobArray()
            };

            JobHandle trimHandle = trimJob.Schedule(points, 64, traceHandle);

            JobHandle disposeHandle = rawHeights.Dispose(trimHandle);
            disposeHandle = filledHeights.Dispose(disposeHandle);
            disposeHandle = flowTarget.Dispose(disposeHandle);
            disposeHandle = order.Dispose(disposeHandle);
            disposeHandle = accumulation.Dispose(disposeHandle);
            disposeHandle = sources.Dispose(disposeHandle);

            return new RegionFetchResult(
                new HydrologyRegionData(points),
                disposeHandle);
        }

        public void Dispose()
        {
            _neighbors8.Dispose();
        }
    }
}