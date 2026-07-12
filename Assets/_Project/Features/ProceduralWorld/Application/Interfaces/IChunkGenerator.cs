using _Project.Features.ProceduralWorld.Domain;
using _Project.Features.ProceduralWorld.Domain.Chunks;

namespace _Project.Features.ProceduralWorld.Application.Interfaces
{
    public interface IChunkGenerator
    {
        GenerationTask Schedule(
            ChunkGenerationRequest request);
    }
}