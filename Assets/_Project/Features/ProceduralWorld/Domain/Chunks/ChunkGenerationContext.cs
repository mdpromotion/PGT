using Unity.Mathematics;

namespace _Project.Features.ProceduralWorld.Domain.Chunks
{
    public readonly struct ChunkGenerationContext
    {
        public readonly ChunkCoordinate Coordinate;
        

        public ChunkGenerationContext(
            ChunkCoordinate coordinate)
        {
            Coordinate = coordinate;
        }
    }
}