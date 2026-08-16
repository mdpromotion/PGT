using Unity.Mathematics;

namespace _Project.Features.ProceduralWorld.Infrastructure.Vegetation
{
    public static class ForestMaskSampler
    {
        public struct Parameters
        {
            public float ForestRegionScale;
            public float ForestCoverage;
            public int ForestRegionOctaves;

            public float PatchScale;
            public int PatchOctaves;
            public float PatchDetailInfluence;
            
            public float Threshold;
        }

        public static float Sample(float2 origin, in Parameters p)
        {
            float2 pMacro = origin / p.ForestRegionScale;
            float macro = FractalNoise01(pMacro, p.ForestRegionOctaves);
            
            float biasExponent = math.lerp(2f, 0.35f, p.ForestCoverage);
            macro = math.pow(macro, biasExponent);

            float2 pMicro = (origin + 137f) / p.PatchScale;
            float micro = FractalNoise01(pMicro, p.PatchOctaves);
            
            const float edgeBand = 0.25f;
            float distanceFromThreshold = math.abs(macro - p.Threshold);
            float edgeFactor = math.saturate(1f - distanceFromThreshold / edgeBand);

            float erosion = (1f - micro) * p.PatchDetailInfluence * edgeFactor;

            return macro - erosion;
        }

        private static float FractalNoise01(float2 p, int octaves)
        {
            float sum = 0f;
            float amplitude = 1f;
            float amplitudeSum = 0f;
            float frequency = 1f;

            for (int i = 0; i < octaves; i++)
            {
                sum += noise.snoise(p * frequency) * amplitude;
                amplitudeSum += amplitude;

                amplitude *= 0.5f;
                frequency *= 2f;
            }

            float normalized = sum / amplitudeSum;
            return normalized * 0.5f + 0.5f;
        }
    }
}