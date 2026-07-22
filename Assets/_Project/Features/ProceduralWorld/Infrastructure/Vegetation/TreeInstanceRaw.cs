using Unity.Mathematics;

namespace _Project.Features.ProceduralWorld.Domain.Landscape
{
    public readonly struct TreeInstanceRaw
    {
        public readonly float2 LocalPosition;
        public readonly float HeightSample;
        public readonly uint Seed;
        public readonly byte PrototypeIndex;

        public TreeInstanceRaw(
            float2 localPosition,
            float heightSample,
            uint seed,
            byte prototypeIndex)
        {
            LocalPosition = localPosition;
            HeightSample = heightSample;
            Seed = seed;
            PrototypeIndex = prototypeIndex;
        }
    }
}