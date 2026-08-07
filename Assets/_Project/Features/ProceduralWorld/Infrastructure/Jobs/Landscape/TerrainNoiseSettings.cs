using Unity.Mathematics;

namespace _Project.Features.ProceduralWorld.Infrastructure.Jobs.Landscape
{
    public struct TerrainNoiseSettings
    {
        public float Scale;
        public int Octaves;
        public float Persistence;
        public float Lacunarity;

        public float RedistributionPower;

        public float2 Offset;
    }
}