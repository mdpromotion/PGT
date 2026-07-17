using _Project.Features.ProceduralWorld.Infrastructure.Hydrology;
using Unity.Collections;

namespace _Project.Features.ProceduralWorld.Infrastructure.Hydrology
{
    public enum HydrologyPointKind : byte
    {
        River = 0
    }

    public readonly struct HydrologyRegionData
    {
        public readonly NativeList<float2Point> Points;

        public HydrologyRegionData(NativeList<float2Point> points)
        {
            Points = points;
        }

        public NativeArray<float2Point> AsDeferredJobArray() =>
            Points.AsDeferredJobArray();

        public void Dispose()
        {
            if (Points.IsCreated)
                Points.Dispose();
        }
    }

    public struct float2Point
    {
        public float X;
        public float Z;

        public float Height;
        public float Strength;

        public int SegmentId;

        public HydrologyPointKind Kind;
    }
}