using _Project.Features.ProceduralWorld.Domain;
using UnityEngine;

namespace _Project.Features.ProceduralWorld.Application
{
    public interface ITerrainWriter
    {
        void Write(
            Terrain terrain,
            ChunkGenerationResult result);
    }
}