using System;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using _Project.Features.ProceduralWorld.Application.Chunks.Generation;
using _Project.Features.ProceduralWorld.Domain;
using _Project.Features.ProceduralWorld.Domain.Chunks;
using _Project.Features.ProceduralWorld.Infrastructure.Jobs;
using _Project.Features.ProceduralWorld.Infrastructure.Jobs.Settings;


namespace _Project.Features.ProceduralWorld.Infrastructure.Hydrology
{
    public sealed class HydrologyGenerator :
        IGenerationStage,
        IDisposable
    {
        private readonly ChunkGrid _grid;
        private readonly HydrologySettings _settings;

        private readonly HydrologyRegionCache _regionCache;



        public HydrologyGenerator(
            ChunkGrid grid,
            HydrologySettings settings,
            HydrologyRegionCache regionCache)
        {
            _grid = grid;
            _settings = settings;
            _regionCache = regionCache;
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



            // Запрашиваем 9 соседних регионов. Get() не блокирует: если региона
            // ещё нет в кэше, он тут же ставится в очередь на построение.
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
                    merged.AsDeferredJobArray(),
                    _settings);



            JobHandle handle =
                job.Schedule(
                    mask.Length,
                    64,
                    mergeHandle);



            return merged.Dispose(handle);
        }



        public void Dispose()
        {
            _regionCache.Dispose();
        }
    }
}