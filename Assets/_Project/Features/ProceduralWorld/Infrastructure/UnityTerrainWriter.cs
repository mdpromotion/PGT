using UnityEngine;

namespace _Project.Features.ProceduralWorld.Infrastructure
{
    public class UnityTerrainWriter
    {
        public void Apply(TerrainData terrainData, float[,] heights)
        {
            terrainData.SetHeights(0, 0, heights);
        }
    }
}