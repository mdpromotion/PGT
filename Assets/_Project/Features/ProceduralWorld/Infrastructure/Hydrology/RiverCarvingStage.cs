using _Project.Features.ProceduralWorld.Application.Chunks.Generation;
using _Project.Features.ProceduralWorld.Domain.Chunks;
using _Project.Features.ProceduralWorld.Domain.Hydrology;
using _Project.Features.ProceduralWorld.Domain.Landscape;
using _Project.Features.ProceduralWorld.Domain.World;
using _Project.Features.ProceduralWorld.Infrastructure.Jobs.Hydrology;
using Unity.Jobs;

namespace _Project.Features.ProceduralWorld.Infrastructure.Hydrology
{
    public sealed class RiverCarvingStage : IGenerationStage
    {
        private readonly RiverCarvingSettings _settings;

        public RiverCarvingStage(RiverCarvingSettings settings)
        {
            _settings = settings;
        }

        public JobHandle Schedule(
            ChunkGenerationState state,
            JobHandle dependency)
        {
            LandscapeData landscape = state.Landscape;
            HydrologyData hydrology = state.Hydrology;

            var job = new CarveRiverbedsJob
            {
                Accumulation = hydrology.Accumulation,
                AccumulationThreshold = _settings.AccumulationThreshold,
                FalloffRange = _settings.FalloffRange,
                MaxCarveDepth = _settings.MaxCarveDepth,
                Heights = landscape.Heights,
            };

            return job.Schedule(landscape.Heights.Length, 64, dependency);
        }
    }
}