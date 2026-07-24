using _Project.Features.ProceduralWorld.Domain.Chunks;
using _Project.Features.ProceduralWorld.Domain.Hydrology;
using _Project.Features.ProceduralWorld.Domain.Landscape;
using UnityEngine;
 
namespace _Project.Features.ProceduralWorld.Infrastructure.Chunks
{
    public sealed class ChunkInstance
    {
        public ChunkCoordinate Coordinate { get; }
        public LandscapeData Landscape { get; }
        public HydrologyData Hydrology { get; } 
        public Terrain Terrain { get; }
 
        public ChunkInstance(
            ChunkCoordinate coordinate,
            LandscapeData landscape,
            HydrologyData hydrology,
            Terrain terrain)
        {
            Coordinate = coordinate;
            Landscape = landscape;
            Hydrology = hydrology;
            Terrain = terrain;
        }
    }
}