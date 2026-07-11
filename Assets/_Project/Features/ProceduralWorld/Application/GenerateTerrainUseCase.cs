using _Project.Features.ProceduralWorld.Domain;
using _Project.Features.ProceduralWorld.Infrastructure;
using UnityEngine;

namespace _Project.Features.ProceduralWorld.Application
{
    public class GenerateTerrainUseCase
    {
        private readonly IHeightmapGenerator _heightmapGenerator;
        private readonly UnityTerrainWriter _terrainWriter;

        public GenerateTerrainUseCase(
            IHeightmapGenerator heightmapGenerator,
            UnityTerrainWriter terrainWriter)
        {
            _heightmapGenerator = heightmapGenerator;
            _terrainWriter = terrainWriter;
        }

        public void Execute(
            TerrainData terrainData,
            NoiseSettings settings,
            ChunkCoordinate coordinate)
        {
            float[,] heights =
                _heightmapGenerator.Generate(
                    terrainData.heightmapResolution,
                    new Vector2(terrainData.size.x, terrainData.size.z),
                    settings,
                    coordinate);

            _terrainWriter.Apply(terrainData, heights);
        }
    }
}