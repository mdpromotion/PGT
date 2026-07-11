using UnityEngine;

namespace _Project.Features.ProceduralWorld.Domain
{
    public interface IHeightmapGenerator
    {
        float[,] Generate(
            int resolution,
            Vector2 terrainSize,
            NoiseSettings settings,
            Vector2 worldOffset);
    }
}