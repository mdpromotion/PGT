using System;
using System.Collections.Concurrent;
using _Project.Features.ProceduralWorld.Domain;
using _Project.Features.ProceduralWorld.Domain.Chunks;
using _Project.Features.ProceduralWorld.Domain.Hydrology;
using _Project.Features.ProceduralWorld.Infrastructure.Jobs.Hydrology;
using _Project.Features.ProceduralWorld.Infrastructure.Jobs.Landscape;
using _Project.Features.ProceduralWorld.Infrastructure.Landscape;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace _Project.Features.ProceduralWorld.Infrastructure.Hydrology
{
    public sealed class MacroRegionCache : IDisposable
    {
        private readonly MacroGridSettings _settings;
        private readonly TerrainNoiseSettingsProvider _noiseProvider;
        
        private readonly double _chunkSizeX;
        private readonly double _chunkSizeZ;

        private readonly ConcurrentDictionary<MacroRegionCoordinate, Lazy<MacroRegionData>> _regions
            = new ConcurrentDictionary<MacroRegionCoordinate, Lazy<MacroRegionData>>();

        public MacroRegionCache(
            MacroGridSettings settings,
            TerrainNoiseSettingsProvider noiseProvider,
            ChunkGrid chunkGrid)
        {
            _settings = settings;
            _noiseProvider = noiseProvider;
            _chunkSizeX = chunkGrid.ChunkSizeX;
            _chunkSizeZ = chunkGrid.ChunkSizeZ;
        }

        public MacroRegionCoordinate ToRegionCoordinate(ChunkCoordinate chunkCoordinate)
        {
            double2 absoluteOrigin = GenerationSpace.AbsoluteChunkOrigin(
                chunkCoordinate, _chunkSizeX, _chunkSizeZ);

            double tileSize = _settings.TileWorldSize;

            int rx = (int)math.floor(absoluteOrigin.x / tileSize);
            int rz = (int)math.floor(absoluteOrigin.y / tileSize);

            return new MacroRegionCoordinate(rx, rz);
        }

        public MacroRegionData GetOrBuild(MacroRegionCoordinate coordinate)
        {
            Lazy<MacroRegionData> lazy = _regions.GetOrAdd(
                coordinate,
                key => new Lazy<MacroRegionData>(() => Build(key)));

            return lazy.Value;
        }

        private MacroRegionData Build(MacroRegionCoordinate coordinate)
        {
            int paddedSize = _settings.PaddedSize;
            float cellSize = _settings.CellSize;
            
            double2 tileOrigin = new double2(
                coordinate.X * (double)_settings.TileWorldSize,
                coordinate.Y * (double)_settings.TileWorldSize);

            double2 worldOrigin = tileOrigin - new double2(
                _settings.PaddingCells * (double)cellSize,
                _settings.PaddingCells * (double)cellSize);

            var region = new MacroRegionData(coordinate, paddedSize, cellSize, worldOrigin);

            int count = paddedSize * paddedSize;

            TerrainNoiseSettings noiseSettings = _noiseProvider.Create();
            NativeArray<float2> octaveOffsets = _noiseProvider.GetOctaveOffsets(noiseSettings.Octaves);
            
            var heightsJob = new SampleMacroHeightsJob
            {
                PaddedSize = paddedSize,
                CellSize = cellSize,
                WorldOrigin = new float2((float)worldOrigin.x, (float)worldOrigin.y),
                Settings = noiseSettings,
                OctaveOffsets = octaveOffsets,
                Heights = region.Heights,
            };
            var heightsHandle = heightsJob.Schedule(count, 64);

            var fillJob = new PriorityFloodFillJob
            {
                PaddedSize = paddedSize,
                Heights = region.Heights,
            };
            var fillHandle = fillJob.Schedule(heightsHandle);

            var flowJob = new ComputeMacroFlowDirectionsJob
            {
                PaddedSize = paddedSize,
                PaddingCells = _settings.PaddingCells,
                TileCells = _settings.TileCells,
                RiverZoneMargin = _settings.RiverZoneMargin,
                EdgeBiasStrength = _settings.EdgeBiasStrength,
                Heights = region.Heights,
                FlowDirection = region.FlowDirection,
            };
            var flowHandle = flowJob.Schedule(count, 64, fillHandle);

            using (var sortedIndices = new NativeArray<int>(count, Allocator.TempJob))
            {
                var accumulationJob = new ComputeMacroAccumulationJob
                {
                    PaddedSize = paddedSize,
                    Heights = region.Heights,
                    FlowDirection = region.FlowDirection,
                    SortedIndices = sortedIndices,
                    Accumulation = region.Accumulation,
                };
                
                accumulationJob.Schedule(flowHandle).Complete();
            }
            
            var waterLevelJob = new ComputeMacroWaterLevelJob
            {
                PaddedSize = paddedSize,
                RiverAccumulationThreshold = 1.0001f,
                PaddingCells = _settings.PaddingCells,
                Heights = region.Heights,
                Accumulation = region.Accumulation,
                WaterLevels = region.WaterLevels,
            };
            
            waterLevelJob.Schedule().Complete();

            return region;
        }

        public void Dispose()
        {
            foreach (var lazy in _regions.Values)
            {
                if (lazy.IsValueCreated)
                    lazy.Value.Dispose();
            }

            _regions.Clear();
        }
    }
}