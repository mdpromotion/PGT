using _Project.Features.ProceduralWorld.Domain.Chunks;
using UnityEngine;

namespace _Project.Features.ProceduralWorld.Infrastructure.Interfaces
{
    public interface ILandscapeFactory
    {
        Terrain Create(ChunkCoordinate coordinate, Transform parent);
        void Show(Terrain terrain);
        void Release(Terrain terrain);
        void Connect(Terrain terrain, Terrain left, Terrain top, Terrain right, Terrain bottom);
    }
}