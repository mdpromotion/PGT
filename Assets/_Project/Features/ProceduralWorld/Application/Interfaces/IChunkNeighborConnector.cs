using System.Collections.Generic;
using _Project.Features.ProceduralWorld.Domain;
using _Project.Features.ProceduralWorld.Domain.Chunks;
using UnityEngine;

namespace _Project.Features.ProceduralWorld.Application.Interfaces
{
    public interface IChunkNeighborConnector
    {
        void Connect(
            IChunkLookup chunks,
            ChunkCoordinate coordinate);


        void Disconnect(
            IChunkLookup chunks,
            ChunkCoordinate coordinate);
    }
}