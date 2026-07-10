using UnityEngine;

namespace _Project.Features.TerrainGeneration.Domain
{
    public class PerlinHeightmapGenerator : IHeightmapGenerator
    {
        public float[,] Generate(int resolution, NoiseSettings settings, Vector2 worldOffset)
        {
            var heights = new float[resolution, resolution];
            var random = new System.Random(settings.Seed);
            
            var octaveOffsets = new Vector2[settings.Octaves];
            for (int i = 0; i < settings.Octaves; i++)
            {
                octaveOffsets[i] = new Vector2(
                    random.Next(-100000, 100000) + worldOffset.x,
                    random.Next(-100000, 100000) + worldOffset.y
                );
            }

            float maxPossibleHeight = 0f;
            float amplitude = 1f;
            for (int i = 0; i < settings.Octaves; i++)
            {
                maxPossibleHeight += amplitude;
                amplitude *= settings.Persistence;
            }

            for (int y = 0; y < resolution; y++)
            {
                for (int x = 0; x < resolution; x++)
                {
                    amplitude = 1f;
                    float frequency = 1f;
                    float noiseHeight = 0f;

                    for (int o = 0; o < settings.Octaves; o++)
                    {
                        float sampleX = (x + octaveOffsets[o].x) / settings.Scale * frequency;
                        float sampleY = (y + octaveOffsets[o].y) / settings.Scale * frequency;

                        float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                        noiseHeight += perlinValue * amplitude;

                        amplitude *= settings.Persistence;
                        frequency *= settings.Lacunarity;
                    }
                    
                    float normalized = (noiseHeight / maxPossibleHeight + 1f) / 2f;
                    heights[y, x] = normalized;
                }
            }

            return heights;
        }
    }
}