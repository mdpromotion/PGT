using UnityEngine;

namespace _Project.Features.TerrainGeneration.Domain
{
    public interface IHeightmapGenerator
    {
        float[,] Generate(int resolution, NoiseSettings settings, Vector2 worldOffset);
    }
}