using Unity.Jobs;
using _Project.Features.ProceduralWorld.Domain.Chunks;


namespace _Project.Features.ProceduralWorld.Application.Chunks
{
    public sealed class GenerationTask
    {
        public JobHandle Handle { get; }

        public ChunkGenerationState State { get; }


        public bool Cancelled;



        public GenerationTask(
            JobHandle handle,
            ChunkGenerationState state)
        {
            Handle = handle;
            State = state;
        }
    }
}