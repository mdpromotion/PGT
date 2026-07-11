using _Project.Features.ProceduralWorld.Domain;
using UnityEngine;

namespace _Project.Features.ProceduralWorld.Infrastructure
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
                    random.Next(-100000, 100000) + settings.Offset.x,
                    random.Next(-100000, 100000) + settings.Offset.y);
            }

            // Теоретический максимум суммы амплитуд — ОДИНАКОВЫЙ для всех чанков,
            // не зависит от того, что реально попало в этот кусок карты.
            // Это критично для бесшовной стыковки.
            float maxAmplitude = 0f;
            float amp = 1f;
            for (int i = 0; i < settings.Octaves; i++)
            {
                maxAmplitude += amp;
                amp *= settings.Persistence;
            }

            float scale = Mathf.Max(settings.Scale, 0.0001f);

            for (int y = 0; y < resolution; y++)
            {
                for (int x = 0; x < resolution; x++)
                {
                    float worldX = worldOffset.x + x * stepX;
                    float worldY = worldOffset.y + y * stepY;

                    float amplitude = 1f;
                    float frequency = 1f;
                    float noiseHeight = 0f;

                    for (int o = 0; o < settings.Octaves; o++)
                    {
                        float sampleX =
                            (worldX + octaveOffsets[o].x) * frequency / scale;
                        float sampleY =
                            (worldY + octaveOffsets[o].y) * frequency / scale;

                        float value = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;

                        noiseHeight += value * amplitude;

                        amplitude *= settings.Persistence;
                        frequency *= settings.Lacunarity;
                    }

                    // Нормализация по ФИКСИРОВАННОМУ теоретическому диапазону —
                    // одинаковому для любого чанка при данных Octaves/Persistence.
                    // Это гарантирует отсутствие швов.
                    float normalized = (noiseHeight / maxAmplitude + 1f) * 0.5f;
                    normalized = Mathf.Clamp01(normalized);

                    // Redistribution — убирает лишние горы, но формула
                    // одинакова для всех чанков, поэтому швов не создаёт.
                    float redistributed = Mathf.Pow(normalized, settings.RedistributionPower);

                    if (redistributed < settings.SeaLevel)
                    {
                        redistributed = settings.SeaLevel;
                    }

                    heights[y, x] = settings.HeightCurve.Evaluate(redistributed);
                }
            }

            return heights;
        }
    }
}