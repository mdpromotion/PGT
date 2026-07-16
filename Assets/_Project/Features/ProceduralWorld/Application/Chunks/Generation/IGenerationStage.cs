using _Project.Features.ProceduralWorld.Domain.Chunks;
using Unity.Jobs;

namespace _Project.Features.ProceduralWorld.Application.Chunks.Generation
{
    public interface IGenerationStage
    {
        JobHandle Schedule(
            ChunkGenerationState state,
            JobHandle dependency);
    }
}