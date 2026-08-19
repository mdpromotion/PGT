using _Project.Features.ProceduralWorld.Application.Chunks.Generation;
using _Project.Features.ProceduralWorld.Domain;
using _Project.Features.ProceduralWorld.Domain.Chunks;
using _Project.Features.ProceduralWorld.Infrastructure.Jobs.Hydrology;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace _Project.Features.ProceduralWorld.Infrastructure.Hydrology
{
    public sealed class WaterSurfaceStage : IGenerationStage
    {
        private const int EdgePaddingCells = 1;

        private readonly RiverCarvingSettings _settings;
        private readonly ChunkGrid _chunkGrid;

        public WaterSurfaceStage(
            RiverCarvingSettings settings,
            ChunkGrid chunkGrid)
        {
            _settings = settings;
            _chunkGrid = chunkGrid;
        }

        public JobHandle Schedule(
            ChunkGenerationState state,
            JobHandle dependency)
        {
            int resolution = state.Context.Resolution;
            int count = resolution * resolution;

            var carveJob = new CarveRiverbedsJob
            {
                Resolution = resolution,
                AccumulationThreshold = _settings.AccumulationThreshold,
                FalloffRange = _settings.FalloffRange,
                MaxCarveDepth = _settings.MaxCarveDepth,

                Accumulation = state.Hydrology.Accumulation,
                WaterSurfaceHeight = state.Hydrology.WaterSurfaceHeight,
                Heights = state.Landscape.Heights,
                RiverMask = state.Hydrology.RiverMask,
            };

            JobHandle carveHandle =
                carveJob.Schedule(count, 64, dependency);
            
            state.WaterMaskPixels =
                new NativeArray<Color32>(
                    count,
                    Allocator.Persistent);

            var packJob = new PackWaterSurfaceTextureJob
            {
                RiverMask = state.Hydrology.RiverMask,
                WaterSurfaceHeight = state.Hydrology.WaterSurfaceHeight,
                MaskPixels = state.WaterMaskPixels,
            };

            JobHandle packHandle =
                packJob.Schedule(count, 64, carveHandle);

            state.WaterBounds =
                new NativeArray<int>(
                    5,
                    Allocator.Persistent);

            state.WaterAverageHeight =
                new NativeArray<float>(
                    1,
                    Allocator.Persistent);

            var boundsJob = new ComputeWaterBoundsJob
            {
                RiverMask = state.Hydrology.RiverMask,
                WaterSurfaceHeight =
                    state.Hydrology.WaterSurfaceHeight,

                Resolution = resolution,
                Padding = EdgePaddingCells,

                Bounds = state.WaterBounds,
                AverageHeight = state.WaterAverageHeight,
            };

            JobHandle boundsHandle =
                boundsJob.Schedule(carveHandle);

            return JobHandle.CombineDependencies(
                boundsHandle,
                packHandle,
                boundsHandle);
        }
    }
}