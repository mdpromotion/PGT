using _Project.Features.ProceduralWorld.Infrastructure.Jobs.Landscape;
using Unity.Collections;
using Unity.Mathematics;

namespace _Project.Features.ProceduralWorld.Infrastructure.Jobs
{
    public static class HeightSampler
    {
        public static float Sample(
            double2 worldPos,
            in TerrainNoiseSettings settings,
            in NativeArray<float2> octaveOffsets)
        {
            double amplitude = 1.0;
            double frequency = 1.0;
            double height = 0.0;
            double maxAmplitude = 0.0;

            double scale =
                math.max(
                    (double)settings.Scale,
                    0.0001);

            int octaveCount =
                math.min(
                    settings.Octaves,
                    octaveOffsets.Length);

            for (int i = 0; i < octaveCount; i++)
            {
                maxAmplitude += amplitude;

                double2 sample =
                    worldPos
                    + (double2)octaveOffsets[i];

                sample *= frequency / scale;

                double value =
                    SimplexNoise2D(sample);

                height += value * amplitude;

                amplitude *= settings.Persistence;
                frequency *= settings.Lacunarity;
            }

            height /=
                math.max(
                    maxAmplitude,
                    0.0001);

            height =
                (height + 1.0) * 0.5;

            height =
                math.clamp(
                    height,
                    0.0,
                    1.0);

            height =
                math.pow(
                    height,
                    settings.RedistributionPower);

            return (float)height;
        }

        private static double SimplexNoise2D(
            double2 v)
        {
            const double c0 = 0.21132486540518713447;
            const double c1 = 0.36602540378443864678;
            const double c2 = -0.57735026918962576451;
            const double c3 = 0.02439024390243902439;

            double skew = (v.x + v.y) * c1;

            double2 i = math.floor(v + skew);

            double unskew = (i.x + i.y) * c0;

            double2 x0 = v - i + unskew;

            double2 i1 = x0.x > x0.y ? new double2(1.0, 0.0) : new double2(0.0, 1.0);
            double2 x1 = x0 - i1 + c0;

            double2 x2 = x0 + c2;

            i = Mod289(i);

            double3 p = Permute(Permute(i.y + new double3(0.0, i1.y, 1.0)) + i.x + new double3(0.0, i1.x, 1.0));

            double3 m = math.max(0.5 - new double3(math.dot(x0, x0), math.dot(x1, x1), math.dot(x2, x2)), 0.0);

            m *= m;
            m *= m;

            double3 x = 2.0 * Fraction(p * c3) - 1.0;

            double3 h = math.abs(x) - 0.5;

            double3 ox = math.floor(x + 0.5);

            double3 a0 = x - ox;

            m *= TaylorInvSqrt(a0 * a0 + h * h);

            double3 g = new double3(a0.x * x0.x + h.x * x0.y, a0.y * x1.x + h.y * x1.y, a0.z * x2.x + h.z * x2.y);

            return 130.0 * math.dot(m, g);
        }

        private static double2 Mod289(double2 x)
        {
            return x - math.floor(x / 289.0) * 289.0;
        }

        private static double3 Mod289(double3 x)
        {
            return x - math.floor(x / 289.0) * 289.0;
        }

        private static double3 Permute(double3 x)
        {
            return Mod289(((x * 34.0) + 1.0) * x);
        }

        private static double3 TaylorInvSqrt(double3 r)
        {
            return 1.79284291400159 - r * 0.85373472095314;
        }

        private static double3 Fraction(double3 x)
        {
            return x - math.floor(x);
        }
    }
}