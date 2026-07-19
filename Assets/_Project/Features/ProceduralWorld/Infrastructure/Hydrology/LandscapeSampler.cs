using Unity.Collections;
using Unity.Mathematics;

namespace _Project.Features.ProceduralWorld.Infrastructure.Hydrology
{
    public static class LandscapeSampler
    {
        public static float SampleBilinear(
            NativeArray<float> source,
            int resolution,
            float x,
            float z)
        {
            int x0 = math.clamp((int)x, 0, resolution - 1);
            int z0 = math.clamp((int)z, 0, resolution - 1);
            int x1 = math.clamp(x0 + 1, 0, resolution - 1);
            int z1 = math.clamp(z0 + 1, 0, resolution - 1);

            float tx = x - x0;
            float tz = z - z0;

            float a = source[z0 * resolution + x0];
            float b = source[z0 * resolution + x1];
            float c = source[z1 * resolution + x0];
            float d = source[z1 * resolution + x1];

            float ab = math.lerp(a, b, tx);
            float cd = math.lerp(c, d, tx);

            return math.lerp(ab, cd, tz);
        }
    }
}