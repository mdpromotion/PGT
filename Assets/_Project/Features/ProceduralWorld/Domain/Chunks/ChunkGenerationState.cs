using _Project.Features.ProceduralWorld.Domain.Landscape;

namespace _Project.Features.ProceduralWorld.Domain.Chunks
{
    public sealed class ChunkGenerationState
    {
        public ChunkGenerationContext Context { get; }

        public LandscapeData Landscape { get; set; }



        public ChunkGenerationState(
            ChunkGenerationContext context)
        {
            Context = context;
        }
    }
}