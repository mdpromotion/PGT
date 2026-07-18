using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using _Project.Features.ProceduralWorld.Application.Chunks.Generation;
using _Project.Features.ProceduralWorld.Application.Interfaces;
using _Project.Features.ProceduralWorld.Domain;
using _Project.Features.ProceduralWorld.Domain.Chunks;
using _Project.Features.ProceduralWorld.Infrastructure.Jobs;
using _Project.Features.ProceduralWorld.Infrastructure.Jobs.Hydrology;
using _Project.Features.ProceduralWorld.Infrastructure.Jobs.Settings;

namespace _Project.Features.ProceduralWorld.Infrastructure.Hydrology
{
    public sealed class HydrologyGenerator :
        IGenerationStage,
        IGenerationCacheEvictor,
        IDisposable
    {
        private readonly struct MergedEntry
        {
            public readonly NativeList<RiverSegment> Segments;
            public readonly SpatialHashData Hash;
            public readonly JobHandle Handle;

            public MergedEntry(
                NativeList<RiverSegment> segments,
                SpatialHashData hash,
                JobHandle handle)
            {
                Segments = segments;
                Hash = hash;
                Handle = handle;
            }
        }

        private readonly ChunkGrid _grid;
        private readonly HydrologySettings _settings;

        private readonly HydrologyRegionCache _regionCache;

        private readonly Dictionary<RegionCoordinate, MergedEntry> _mergedCache = new();
        private readonly List<RegionCoordinate> _mergedEvictionBuffer = new();

        public HydrologyGenerator(
            ChunkGrid grid,
            HydrologySettings settings,
            HydrologyRegionCache regionCache)
        {
            _grid = grid;
            _settings = settings;
            _regionCache = regionCache;
        }

        public void EvictOutside(
            ChunkCoordinate center,
            int viewDistance)
        {
            RegionCoordinate centerRegion =
                RegionCoordinate.FromChunk(
                    center,
                    _settings.RegionSizeInChunks);

            int keepRegionRadius =
                (viewDistance / _settings.RegionSizeInChunks) + 1;

            EvictMergedOutside(centerRegion, keepRegionRadius);

            _regionCache.EvictOutside(centerRegion, keepRegionRadius + 1);
        }

        private void EvictMergedOutside(
            RegionCoordinate center,
            int keepRadius)
        {
            _mergedEvictionBuffer.Clear();

            foreach (RegionCoordinate key in _mergedCache.Keys)
            {
                int dx = key.X - center.X;
                int dy = key.Y - center.Y;

                int chebyshev = System.Math.Max(
                    System.Math.Abs(dx),
                    System.Math.Abs(dy));

                if (chebyshev > keepRadius)
                {
                    _mergedEvictionBuffer.Add(key);
                }
            }

            foreach (RegionCoordinate key in _mergedEvictionBuffer)
            {
                MergedEntry entry = _mergedCache[key];

                entry.Handle.Complete();
                entry.Segments.Dispose();
                entry.Hash.Dispose();

                _mergedCache.Remove(key);
            }
        }

        public JobHandle Schedule(
            ChunkGenerationState state,
            JobHandle dependency)
        {
            ChunkGenerationContext context =
                state.Context;

            RegionCoordinate region =
                RegionCoordinate.FromChunk(
                    context.Coordinate,
                    _settings.RegionSizeInChunks);

            if (!_mergedCache.TryGetValue(region, out MergedEntry merged))
            {
                merged = BuildNeighbourhood(region, dependency);
                _mergedCache.Add(region, merged);
            }

            JobHandle mergeHandle =
                JobHandle.CombineDependencies(dependency, merged.Handle);

            NativeArray<float> mask =
                new NativeArray<float>(
                    context.Resolution *
                    context.Resolution,
                    Allocator.Persistent);

            state.Landscape.AttachRiverMask(mask);

            HydrologyCarveJob job =
                new HydrologyCarveJob(
                    state.Landscape.Heights,
                    mask,
                    context.Resolution,
                    _grid.ChunkSizeX,
                    _grid.ChunkSizeZ,
                    new int2(
                        context.Coordinate.X,
                        context.Coordinate.Y),
                    merged.Segments.AsDeferredJobArray(),
                    merged.Hash,
                    _settings);

            return job.Schedule(
                mask.Length,
                64,
                mergeHandle);
        }

        private MergedEntry BuildNeighbourhood(
            RegionCoordinate region,
            JobHandle dependency)
        {
            NativeArray<float2Point> source0 = default;
            NativeArray<float2Point> source1 = default;
            NativeArray<float2Point> source2 = default;
            NativeArray<float2Point> source3 = default;
            NativeArray<float2Point> source4 = default;
            NativeArray<float2Point> source5 = default;
            NativeArray<float2Point> source6 = default;
            NativeArray<float2Point> source7 = default;
            NativeArray<float2Point> source8 = default;

            JobHandle regionsHandle = dependency;

            int slot = 0;

            for (int dz = -1; dz <= 1; dz++)
            {
                for (int dx = -1; dx <= 1; dx++)
                {
                    RegionCoordinate neighbour =
                        new RegionCoordinate(
                            region.X + dx,
                            region.Y + dz);

                    RegionFetchResult fetch =
                        _regionCache.Get(neighbour);

                    NativeArray<float2Point> deferred =
                        fetch.Data.AsDeferredJobArray();

                    switch (slot)
                    {
                        case 0: source0 = deferred; break;
                        case 1: source1 = deferred; break;
                        case 2: source2 = deferred; break;
                        case 3: source3 = deferred; break;
                        case 4: source4 = deferred; break;
                        case 5: source5 = deferred; break;
                        case 6: source6 = deferred; break;
                        case 7: source7 = deferred; break;
                        case 8: source8 = deferred; break;
                    }

                    slot++;

                    regionsHandle = JobHandle.CombineDependencies(
                        regionsHandle,
                        fetch.Handle);
                }
            }

            NativeList<float2Point> merged =
                new NativeList<float2Point>(Allocator.Persistent);

            MergeRegionsJob mergeJob = new MergeRegionsJob
            {
                Source0 = source0,
                Source1 = source1,
                Source2 = source2,
                Source3 = source3,
                Source4 = source4,
                Source5 = source5,
                Source6 = source6,
                Source7 = source7,
                Source8 = source8,
                Output = merged
            };

            JobHandle mergeHandle =
                mergeJob.Schedule(regionsHandle);

            int initialCapacity = math.max(
                1,
                _settings.RiverSourceCount *
                (_settings.MaxTraceSteps + 1) * 2);

            NativeList<RiverSegment> segments =
                new NativeList<RiverSegment>(initialCapacity, Allocator.Persistent);

            BuildRiverSegmentsJob segmentJob = new BuildRiverSegmentsJob
            {
                Points = merged.AsDeferredJobArray(),
                Segments = segments
            };

            JobHandle segmentHandle =
                segmentJob.Schedule(mergeHandle);

            JobHandle pointsDisposeHandle =
                merged.Dispose(segmentHandle);

            float regionWorldSizeX =
                _grid.ChunkSizeX * _settings.RegionSizeInChunks;

            float regionWorldSizeZ =
                _grid.ChunkSizeZ * _settings.RegionSizeInChunks;

            float2 origin = new float2(
                (region.X - 1) * regionWorldSizeX,
                (region.Y - 1) * regionWorldSizeZ);

            float cellSize = math.max(
                _settings.RiverWidth * 3f,
                0.001f);

            int gridWidth = (int)math.ceil(3f * regionWorldSizeX / cellSize) + 1;
            int gridHeight = (int)math.ceil(3f * regionWorldSizeZ / cellSize) + 1;

            NativeArray<int> cellStart =
                new NativeArray<int>(gridWidth * gridHeight, Allocator.Persistent);

            NativeArray<int> cellCount =
                new NativeArray<int>(gridWidth * gridHeight, Allocator.Persistent);

            NativeList<int> pointIndices =
                new NativeList<int>(Allocator.Persistent);

            BuildSpatialHashJob hashJob = new BuildSpatialHashJob
            {
                Segments = segments.AsDeferredJobArray(),
                Origin = origin,
                CellSize = cellSize,
                GridWidth = gridWidth,
                GridHeight = gridHeight,
                CellStart = cellStart,
                CellCount = cellCount,
                PointIndices = pointIndices
            };

            JobHandle hashHandle =
                hashJob.Schedule(segmentHandle);

            SpatialHashData hash = new SpatialHashData(
                cellStart,
                cellCount,
                pointIndices,
                origin,
                cellSize,
                gridWidth,
                gridHeight);

            JobHandle finalHandle =
                JobHandle.CombineDependencies(hashHandle, pointsDisposeHandle);

            return new MergedEntry(segments, hash, finalHandle);
        }

        public void Dispose()
        {
            foreach (MergedEntry entry in _mergedCache.Values)
            {
                entry.Handle.Complete();
                entry.Segments.Dispose();
                entry.Hash.Dispose();
            }

            _mergedCache.Clear();

            _regionCache.Dispose();
        }
    }
}