using _Project.Features.ProceduralWorld.Domain.Landscape;
using UnityEngine;

namespace _Project.Features.ProceduralWorld.Application.Interfaces
{
    public interface ITerrainWriter
    {
        void Write(
            Terrain terrain,
            LandscapeData data);
    }
}