using _Project.Features.ProceduralWorld.Domain.Chunks;
using Unity.Jobs;

namespace _Project.Features.ProceduralWorld.Domain
{
    public class GenerationTask
    {
        public JobHandle Handle;

        public ChunkGenerationResult Result;

        public bool Cancelled;


        public GenerationTask(
            JobHandle handle,
            ChunkGenerationResult result)
        {
            Handle = handle;
            Result = result;
            Cancelled = false;
        }
    }
}