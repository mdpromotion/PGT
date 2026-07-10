using _Project.Features.TerrainGeneration.Domain;
using _Project.Features.TerrainGeneration.Infrastructure;
using UnityEngine;

namespace _Project.Features.TerrainGeneration.Application
{
    public class GenerateTerrainGridUseCase
    {
        private readonly GenerateTerrainUseCase _generateTerrainUseCase; // переиспользуем
        private readonly TerrainChunkFactory _chunkFactory;

        public GenerateTerrainGridUseCase(
            GenerateTerrainUseCase generateTerrainUseCase,
            TerrainChunkFactory chunkFactory)
        {
            _generateTerrainUseCase = generateTerrainUseCase;
            _chunkFactory = chunkFactory;
        }

        public void Execute(
            Terrain centerTerrain,
            NoiseSettings noiseSettings,
            Vector2 worldOffset,
            ChunkGridSettings gridSettings,
            Transform chunksParent)
        {
            int resolution = centerTerrain.terrainData.heightmapResolution;
            int radius = gridSettings.Radius;

            var grid = new Terrain[2 * radius + 1, 2 * radius + 1];

            for (int cz = -radius; cz <= radius; cz++)
            {
                for (int cx = -radius; cx <= radius; cx++)
                {
                    Terrain chunkTerrain = (cx == 0 && cz == 0)
                        ? centerTerrain
                        : _chunkFactory.CreateChunk(centerTerrain, cx, cz, chunksParent);

                    Vector2 chunkOffset = worldOffset + new Vector2(
                        cx * (resolution - 1),
                        cz * (resolution - 1)
                    );

                    _generateTerrainUseCase.Execute(chunkTerrain.terrainData, noiseSettings, chunkOffset);

                    grid[cx + radius, cz + radius] = chunkTerrain;
                }
            }

            StitchNeighbours(grid, radius);
        }

        private void StitchNeighbours(Terrain[,] grid, int radius)
        {
            int size = 2 * radius + 1;
            for (int z = 0; z < size; z++)
            {
                for (int x = 0; x < size; x++)
                {
                    Terrain left = x > 0 ? grid[x - 1, z] : null;
                    Terrain right = x < size - 1 ? grid[x + 1, z] : null;
                    Terrain bottom = z > 0 ? grid[x, z - 1] : null;
                    Terrain top = z < size - 1 ? grid[x, z + 1] : null;

                    grid[x, z].SetNeighbors(left, top, right, bottom);
                }
            }
        }
    }
}