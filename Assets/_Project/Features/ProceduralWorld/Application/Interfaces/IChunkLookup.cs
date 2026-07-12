using _Project.Features.ProceduralWorld.Domain;
using UnityEngine;

namespace _Project.Features.ProceduralWorld.Application.Interfaces
{
    public interface IChunkLookup
    {
        Terrain Get(
            ChunkCoordinate coordinate);
    }
}