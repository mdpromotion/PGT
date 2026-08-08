using Unity.Mathematics;

namespace _Project.Features.ProceduralWorld.Domain.Vegetation
{
    public readonly struct VegetationInstanceData
    {
        public readonly float3 WorldPosition;
        public readonly float SlopeDegrees;
        public readonly float NormalizedHeight01;
        public readonly uint RandomSeed;

        public VegetationInstanceData(
            float3 worldPosition,
            float slopeDegrees,
            float normalizedHeight01,
            uint randomSeed)
        {
            WorldPosition = worldPosition;
            SlopeDegrees = slopeDegrees;
            NormalizedHeight01 = normalizedHeight01;
            RandomSeed = randomSeed;
        }
    }
}