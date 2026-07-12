using Unity.Collections;
using Unity.Mathematics;

namespace _Project.Features.ProceduralWorld.Infrastructure.Jobs
{
    public struct NoiseJobSettings
    {
        public float Scale;

        public int Octaves;

        public float Persistence;

        public float Lacunarity;

        public float RedistributionPower;

        public float SeaLevel;

        public float HeightMultiplier;

        public float2 Offset;

        public FixedList128Bytes<float2> OctaveOffsets;
    }
}