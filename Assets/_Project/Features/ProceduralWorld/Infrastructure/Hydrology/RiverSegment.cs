using Unity.Mathematics;

namespace _Project.Features.ProceduralWorld.Infrastructure.Hydrology
{
    public struct RiverSegment
    {
        public float2 A;
        public float2 B;

        public float HeightA;
        public float HeightB;

        public float StrengthA;
        public float StrengthB;
        
        public float LeftBankHeightA;
        public float LeftBankHeightB;
        public float RightBankHeightA;
        public float RightBankHeightB;

        public int SegmentId;
    }
}