using _Project.Features.ProceduralWorld.Application.Chunks.Generation;
using _Project.Features.ProceduralWorld.Domain.Chunks;
using _Project.Features.ProceduralWorld.Domain.Hydrology;
using _Project.Features.ProceduralWorld.Infrastructure.Jobs.Hydrology;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace _Project.Features.ProceduralWorld.Infrastructure.Hydrology
{
    public sealed class WaterSurfaceStage : IGenerationStage
    {
        private readonly RiverCarvingSettings _settings;

        public WaterSurfaceStage(RiverCarvingSettings settings)
        {
            _settings = settings;
        }

        public JobHandle Schedule(
            ChunkGenerationState state,
            JobHandle dependency)
        {
            int resolution = state.Context.Resolution;
            int count = resolution * resolution;

            var carveJob = new CarveRiverbedsJob
            {
                Accumulation = state.Hydrology.Accumulation,
                AccumulationThreshold = _settings.AccumulationThreshold,
                FalloffRange = _settings.FalloffRange,
                MaxCarveDepth = _settings.MaxCarveDepth,
                Heights = state.Landscape.Heights,
            };

            JobHandle carveHandle = carveJob.Schedule(count, 64, dependency);

            NativeArray<Color32> pixels = new NativeArray<Color32>(count, Allocator.Persistent);

            var surfaceJob = new GenerateWaterSurfaceJob
            {
                Accumulation = state.Hydrology.Accumulation,
                AccumulationThreshold = _settings.AccumulationThreshold,
                FalloffRange = _settings.FalloffRange,
                Pixels = pixels
            };

            JobHandle surfaceHandle = surfaceJob.Schedule(count, 64, carveHandle);

            state.WaterSurface = new WaterSurfaceData(pixels, resolution);

            return surfaceHandle;
        }
    }
}