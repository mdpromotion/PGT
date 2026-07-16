namespace _Project.Features.ProceduralWorld.Domain.Chunks
{
    public interface IChunkGenerationStage
    {
        void Execute(
            ChunkGenerationState state);
    }
}