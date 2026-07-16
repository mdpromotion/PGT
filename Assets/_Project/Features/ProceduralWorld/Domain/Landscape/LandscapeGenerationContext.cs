using Unity.Mathematics;
using _Project.Features.ProceduralWorld.Domain.Chunks;

namespace _Project.Features.ProceduralWorld.Domain.Landscape
{
    public readonly struct LandscapeGenerationContext
    {
        public readonly ChunkCoordinate Coordinate;

        public readonly float2 WorldPosition;

        public readonly float SizeX;

        public readonly float SizeZ;

        public readonly int Resolution;


        public LandscapeGenerationContext(
            ChunkCoordinate coordinate,
            float2 worldPosition,
            float sizeX,
            float sizeZ,
            int resolution)
        {
            Coordinate = coordinate;
            WorldPosition = worldPosition;
            SizeX = sizeX;
            SizeZ = sizeZ;
            Resolution = resolution;
        }
    }
}