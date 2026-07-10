using _Project.Features.TerrainGeneration.Domain;
using UnityEngine;

namespace _Project.Features.TerrainGeneration.Infrastructure
{
    public class PerlinHeightmapGenerator : IHeightmapGenerator
    {
        public float[,] Generate(
            int resolution,
            Vector2 terrainSize,
            NoiseSettings settings,
            Vector2 worldOffset)
        {
            float[,] heights = new float[resolution, resolution];

            float stepX = terrainSize.x / (resolution - 1);
            float stepY = terrainSize.y / (resolution - 1);

            System.Random random = new System.Random(settings.Seed);

            Vector2[] octaveOffsets = new Vector2[settings.Octaves];

            for (int i = 0; i < settings.Octaves; i++)
            {
                octaveOffsets[i] = new Vector2(
                    random.Next(-100000,100000),
                    random.Next(-100000,100000));
            }

            float maxHeight = 0;
            float amplitude = 1;

            for (int i = 0; i < settings.Octaves; i++)
            {
                maxHeight += amplitude;
                amplitude *= settings.Persistence;
            }

            for (int y = 0; y < resolution; y++)
            {
                for (int x = 0; x < resolution; x++)
                {
                    float worldX = worldOffset.x + x * stepX;
                    float worldY = worldOffset.y + y * stepY;

                    float noiseHeight = 0;

                    amplitude = 1;
                    float frequency = 1;

                    for (int o = 0; o < settings.Octaves; o++)
                    {
                        float sampleX =
                            (worldX + octaveOffsets[o].x)
                            * frequency
                            / settings.Scale;

                        float sampleY =
                            (worldY + octaveOffsets[o].y)
                            * frequency
                            / settings.Scale;

                        float value =
                            Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;

                        noiseHeight += value * amplitude;

                        amplitude *= settings.Persistence;
                        frequency *= settings.Lacunarity;
                    }

                    heights[y, x] =
                        Mathf.InverseLerp(
                            -maxHeight,
                            maxHeight,
                            noiseHeight);
                }
            }

            return heights;
        }
    }
}