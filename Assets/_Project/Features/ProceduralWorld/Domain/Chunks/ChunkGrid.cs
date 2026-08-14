using _Project.Features.ProceduralWorld.Domain.Chunks;
using UnityEngine;

namespace _Project.Features.ProceduralWorld.Domain
{
    public class ChunkGrid
    {
        public float ChunkSizeX { get; }
        public float ChunkSizeZ { get; }

        public ChunkCoordinate OriginCoordinate { get; private set; }

        public ChunkGrid(float chunkSizeX, float chunkSizeZ)
        {
            ChunkSizeX = chunkSizeX;
            ChunkSizeZ = chunkSizeZ;
            OriginCoordinate = new ChunkCoordinate(0, 0);
        }

        public Vector2 ToWorldOffset(ChunkCoordinate coordinate)
        {
            return new Vector2(
                (coordinate.X - OriginCoordinate.X) * ChunkSizeX,
                (coordinate.Y - OriginCoordinate.Y) * ChunkSizeZ);
        }

        public ChunkCoordinate ToChunkCoordinate(Vector3 worldPosition)
        {
            int relativeX = Mathf.FloorToInt(worldPosition.x / ChunkSizeX);
            int relativeY = Mathf.FloorToInt(worldPosition.z / ChunkSizeZ);

            return new ChunkCoordinate(
                relativeX + OriginCoordinate.X,
                relativeY + OriginCoordinate.Y);
        }

        public void SetOriginCoordinate(ChunkCoordinate origin)
        {
            OriginCoordinate = origin;
        }
    }
}