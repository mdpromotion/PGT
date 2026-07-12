using System.Collections.Generic;
using _Project.Features.ProceduralWorld.Application.Interfaces;
using _Project.Features.ProceduralWorld.Domain;
using UnityEngine;

namespace _Project.Features.ProceduralWorld.Application.Chunks
{
    public class ChunkRepository : IChunkLookup
    {
        private readonly Dictionary<
            ChunkCoordinate,
            Terrain> _chunks = new();


        public bool Contains(
            ChunkCoordinate coordinate)
        {
            return _chunks.ContainsKey(coordinate);
        }


        public bool TryGet(
            ChunkCoordinate coordinate,
            out Terrain terrain)
        {
            return _chunks.TryGetValue(
                coordinate,
                out terrain);
        }


        public Terrain Get(
            ChunkCoordinate coordinate)
        {
            return _chunks.TryGetValue(
                coordinate,
                out Terrain terrain)
                ? terrain
                : null;
        }


        public void Add(
            ChunkCoordinate coordinate,
            Terrain terrain)
        {
            _chunks.Add(
                coordinate,
                terrain);
        }


        public void Remove(
            ChunkCoordinate coordinate)
        {
            _chunks.Remove(coordinate);
        }
    }
}