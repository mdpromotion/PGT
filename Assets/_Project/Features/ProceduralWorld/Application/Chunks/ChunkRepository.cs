using System.Collections.Generic;
using _Project.Features.ProceduralWorld.Application.Interfaces;
using _Project.Features.ProceduralWorld.Domain.Chunks;
using _Project.Features.ProceduralWorld.Infrastructure.Chunks;

namespace _Project.Features.ProceduralWorld.Application.Chunks
{
    public class ChunkRepository : IChunkLookup
    {
        private readonly Dictionary<
            ChunkCoordinate,
            ChunkInstance> _chunks = new();



        public bool Contains(
            ChunkCoordinate coordinate)
        {
            return _chunks.ContainsKey(coordinate);
        }



        public bool TryGet(
            ChunkCoordinate coordinate,
            out ChunkInstance chunk)
        {
            return _chunks.TryGetValue(
                coordinate,
                out chunk);
        }



        public ChunkInstance Get(
            ChunkCoordinate coordinate)
        {
            return _chunks.TryGetValue(
                coordinate,
                out ChunkInstance chunk)
                ? chunk
                : null;
        }



        public void Add(
            ChunkInstance chunk)
        {
            _chunks.Add(
                chunk.Coordinate,
                chunk);
        }



        public void Remove(
            ChunkCoordinate coordinate)
        {
            _chunks.Remove(coordinate);
        }
    }
}