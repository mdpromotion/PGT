using Unity.Mathematics;

namespace _Project.Features.ProceduralWorld.Infrastructure.Jobs
{
    public struct TerrainNoiseSettings
    {
        public float Scale;
        public int Octaves;
        public float Persistence;
        public float Lacunarity;
        public float RedistributionPower;
        public float SeaLevel;
        public float HeightMultiplier;
        public float2 Offset;

        public float RiverScale;
        public float RiverWidth;
        public float RiverFrequency;
        public float2 RiverOffset;
    }
}