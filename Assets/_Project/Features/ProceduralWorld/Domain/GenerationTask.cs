using _Project.Features.ProceduralWorld.Domain.Chunks;
using Unity.Jobs;

namespace _Project.Features.ProceduralWorld.Domain
{
    public readonly struct GenerationTask
    {
        public readonly JobHandle Handle;

        public readonly ChunkGenerationResult Result;


        public GenerationTask(
            JobHandle handle,
            ChunkGenerationResult result)
        {
            Handle = handle;
            Result = result;
        }
    }
}