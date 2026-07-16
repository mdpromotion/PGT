using _Project.Features.ProceduralWorld.Domain;
using _Project.Features.ProceduralWorld.Domain.Chunks;
using UnityEngine;

namespace _Project.Features.ProceduralWorld.Presentation
{
    [DisallowMultipleComponent]
    public class TerrainChunkCoordinate: MonoBehaviour
    {
        [SerializeField]
        private int x;

        [SerializeField]
        private int y;

        public ChunkCoordinate Coordinate => new ChunkCoordinate(x, y);

        public void Initialize(ChunkCoordinate value)
        {
            x = value.X;
            y = value.Y;
        }
    }
}