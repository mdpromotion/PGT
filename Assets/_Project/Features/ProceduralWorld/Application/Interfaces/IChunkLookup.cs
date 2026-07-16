using _Project.Features.ProceduralWorld.Domain.Chunks;

namespace _Project.Features.ProceduralWorld.Application.Interfaces
{
    public interface IChunkLookup
    {
        bool Contains(
            ChunkCoordinate coordinate);


        bool TryGet(
            ChunkCoordinate coordinate,
            out ChunkInstance chunk);


        ChunkInstance Get(
            ChunkCoordinate coordinate);
    }
}