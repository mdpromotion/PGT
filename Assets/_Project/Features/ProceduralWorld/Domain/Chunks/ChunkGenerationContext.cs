using Unity.Mathematics;

namespace _Project.Features.ProceduralWorld.Domain.Chunks
{
    public readonly struct ChunkGenerationContext
    {
        public readonly ChunkCoordinate Coordinate;

        public readonly float2 WorldPosition;

        public readonly float ChunkSizeX;

        public readonly float ChunkSizeZ;

        public readonly int Resolution;



        public ChunkGenerationContext(
            ChunkCoordinate coordinate,
            float2 worldPosition,
            float chunkSizeX,
            float chunkSizeZ,
            int resolution)
        {
            Coordinate = coordinate;

            WorldPosition = worldPosition;

            ChunkSizeX = chunkSizeX;

            ChunkSizeZ = chunkSizeZ;

            Resolution = resolution;
        }
    }
}