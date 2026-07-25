using System;
using System.Collections.Concurrent;
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

        private readonly ConcurrentDictionary<MacroRegionCoordinate, Lazy<MacroRegionData>> _regions
            = new ConcurrentDictionary<MacroRegionCoordinate, Lazy<MacroRegionData>>();

        public MacroRegionCache(
            MacroGridSettings settings,
            TerrainNoiseSettingsProvider noiseProvider)
        {
            _settings = settings;
            _noiseProvider = noiseProvider;
        }

        public MacroRegionCoordinate ToRegionCoordinate(float2 worldPos)
        {
            float tileSize = _settings.TileWorldSize;

            int rx = (int)math.floor(worldPos.x / tileSize);
            int rz = (int)math.floor(worldPos.y / tileSize);

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

            float2 tileOrigin = new float2(
                coordinate.X * _settings.TileWorldSize,
                coordinate.Y * _settings.TileWorldSize);

            float2 worldOrigin = tileOrigin - new float2(
                _settings.PaddingCells * cellSize,
                _settings.PaddingCells * cellSize);

            var region = new MacroRegionData(coordinate, paddedSize, cellSize, worldOrigin);

            int count = paddedSize * paddedSize;

            TerrainNoiseSettings noiseSettings = _noiseProvider.Create();
            NativeArray<float2> octaveOffsets = _noiseProvider.GetOctaveOffsets(noiseSettings.Octaves);

            var heightsJob = new SampleMacroHeightsJob
            {
                PaddedSize = paddedSize,
                CellSize = cellSize,
                WorldOrigin = worldOrigin,
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