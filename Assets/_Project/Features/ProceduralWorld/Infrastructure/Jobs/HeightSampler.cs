using _Project.Features.ProceduralWorld.Infrastructure.Jobs.Settings;
using Unity.Collections;
using Unity.Mathematics;

namespace _Project.Features.ProceduralWorld.Infrastructure.Jobs
{
    public static class HeightSampler
    {
        public static float Sample(
            float2 worldPos,
            in TerrainNoiseSettings settings,
            in NativeArray<float2> octaveOffsets)
        {
            float amplitude = 1f;
            float frequency = 1f;
            float height = 0f;
            float maxAmplitude = 0f;

            float scale = math.max(settings.Scale, 0.0001f);
            
            int octaveCount = math.min(settings.Octaves, octaveOffsets.Length);

            for (int i = 0; i < octaveCount; i++)
            {
                maxAmplitude += amplitude;

                float2 sample = worldPos + octaveOffsets[i];
                sample *= frequency / scale;

                float value = noise.snoise(sample);
                height += value * amplitude;

                amplitude *= settings.Persistence;
                frequency *= settings.Lacunarity;
            }

            height = height / math.max(maxAmplitude, 0.0001f);
            height = (height + 1f) * 0.5f;
            height = math.clamp(height, 0f, 1f);
            height = math.pow(height, settings.RedistributionPower);

            return height;
        }
    }
}