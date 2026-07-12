using _Project.Features.ProceduralWorld.Domain;
using UnityEngine;

namespace _Project.Features.ProceduralWorld.Infrastructure
{
    public interface ITerrainFactory
    {
        Terrain Create(
            ChunkCoordinate coordinate,
            Transform parent);

        void Release(
            Terrain terrain);
    }
}