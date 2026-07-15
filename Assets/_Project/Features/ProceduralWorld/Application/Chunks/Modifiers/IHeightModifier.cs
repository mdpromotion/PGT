using _Project.Features.ProceduralWorld.Domain.Chunks;
using Unity.Collections;
using Unity.Jobs;

namespace _Project.Features.ProceduralWorld.Application.Chunks.Modifiers
{
    public interface IHeightModifier
    {
        bool Enabled { get; }


        JobHandle Schedule(
            ChunkGenerationContext context,
            NativeArray<float> heights,
            JobHandle dependency);
    }
}