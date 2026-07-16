using _Project.Features.ProceduralWorld.Domain.Chunks;
using UnityEngine;

namespace _Project.Features.ProceduralWorld.Domain
{
    public class ChunkGrid
    {
        public float ChunkSizeX { get; }
        public float ChunkSizeZ { get; }

        public ChunkGrid(float chunkSizeX, float chunkSizeZ)
        {
            ChunkSizeX = chunkSizeX;
            ChunkSizeZ = chunkSizeZ;
        }

        public Vector2 ToWorldOffset(ChunkCoordinate coordinate)
        {
            return new Vector2(
                coordinate.X * ChunkSizeX,
                coordinate.Y * ChunkSizeZ);
        }

        public ChunkCoordinate ToChunkCoordinate(Vector3 worldPosition)
        {
            return new ChunkCoordinate(
                Mathf.FloorToInt(worldPosition.x / ChunkSizeX),
                Mathf.FloorToInt(worldPosition.z / ChunkSizeZ));
        }
    }
}