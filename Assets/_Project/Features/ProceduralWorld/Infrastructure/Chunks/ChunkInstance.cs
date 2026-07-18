using _Project.Features.ProceduralWorld.Domain.Chunks;
using _Project.Features.ProceduralWorld.Domain.Landscape;
using UnityEngine;

namespace _Project.Features.ProceduralWorld.Infrastructure.Chunks
{
    public sealed class ChunkInstance
    {
        public ChunkCoordinate Coordinate { get; }

        public LandscapeData Landscape { get; }

        public Terrain Terrain { get; }


        public ChunkInstance(
            ChunkCoordinate coordinate,
            LandscapeData landscape,
            Terrain terrain)
        {
            Coordinate = coordinate;

            Landscape = landscape;

            Terrain = terrain;
        }
    }
}