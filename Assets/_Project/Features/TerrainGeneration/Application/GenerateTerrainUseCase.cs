using _Project.Features.TerrainGeneration.Domain;
using _Project.Features.TerrainGeneration.Infrastructure;
using UnityEngine;

namespace _Project.Features.TerrainGeneration.Application
{
    public class GenerateTerrainUseCase
    {
        private readonly IHeightmapGenerator _heightmapGenerator;
        private readonly UnityTerrainWriter _terrainWriter;

        public GenerateTerrainUseCase(IHeightmapGenerator heightmapGenerator, UnityTerrainWriter terrainWriter)
        {
            _heightmapGenerator = heightmapGenerator;
            _terrainWriter = terrainWriter;
        }

        public void Execute(
            TerrainData terrainData,
            NoiseSettings settings,
            Vector2 worldOffset)
        {
            float[,] heights =
                _heightmapGenerator.Generate(
                    terrainData.heightmapResolution,
                    new Vector2(
                        terrainData.size.x,
                        terrainData.size.z),
                    settings,
                    worldOffset);

            _terrainWriter.Apply(
                terrainData,
                heights);
        }
    }
}