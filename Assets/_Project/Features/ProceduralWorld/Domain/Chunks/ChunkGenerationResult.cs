namespace _Project.Features.ProceduralWorld.Domain.Chunks
{
    public sealed class ChunkGenerationResult
    {
        public ChunkGenerationState State { get; }


        public ChunkCoordinate Coordinate =>
            State.Context.Coordinate;



        public ChunkGenerationResult(
            ChunkGenerationState state)
        {
            State = state;
        }
    }
}