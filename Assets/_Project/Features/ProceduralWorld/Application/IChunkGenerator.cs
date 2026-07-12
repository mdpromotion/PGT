using _Project.Features.ProceduralWorld.Domain;

namespace _Project.Features.ProceduralWorld.Application
{
    public interface IChunkGenerator
    {
        GenerationTask Schedule(
            ChunkGenerationRequest request);
    }
}