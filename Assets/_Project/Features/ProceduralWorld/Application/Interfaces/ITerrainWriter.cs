using _Project.Features.ProceduralWorld.Domain;
using _Project.Features.ProceduralWorld.Domain.Chunks;
using UnityEngine;

namespace _Project.Features.ProceduralWorld.Application.Interfaces
{
    public interface ITerrainWriter
    {
        void Write(
            Terrain terrain,
            ChunkGenerationResult result);
    }
}