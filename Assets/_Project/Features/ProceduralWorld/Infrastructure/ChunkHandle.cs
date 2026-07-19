using _Project.Features.ProceduralWorld.Infrastructure.Hydrology;
using _Project.Features.ProceduralWorld.Presentation;
using UnityEngine;

namespace _Project.Features.ProceduralWorld.Infrastructure
{
    public readonly struct ChunkHandle
    {
        public readonly TerrainCollider Collider;
        public readonly TerrainChunkCoordinate Marker;
        public readonly MeshRenderer WaterRenderer;
        public readonly WaterState WaterState;

        public ChunkHandle(
            TerrainCollider collider,
            TerrainChunkCoordinate marker,
            MeshRenderer waterRenderer,
            WaterState waterState)
        {
            Collider = collider;
            Marker = marker;
            WaterRenderer = waterRenderer;
            WaterState = waterState;
        }
    }
}