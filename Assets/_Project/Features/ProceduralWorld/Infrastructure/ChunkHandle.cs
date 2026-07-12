using _Project.Features.ProceduralWorld.Presentation;
using UnityEngine;

namespace _Project.Features.ProceduralWorld.Infrastructure
{
    public readonly struct ChunkHandle
    {
        public readonly TerrainCollider Collider;
        public readonly TerrainChunkCoordinate Marker;

        public ChunkHandle(TerrainCollider collider, TerrainChunkCoordinate marker)
        {
            Collider = collider;
            Marker = marker;
        }
    }
}