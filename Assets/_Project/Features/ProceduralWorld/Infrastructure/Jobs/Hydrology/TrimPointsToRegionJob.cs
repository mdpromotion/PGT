using _Project.Features.ProceduralWorld.Infrastructure.Hydrology;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

namespace _Project.Features.ProceduralWorld.Infrastructure.Jobs.Hydrology
{
    [BurstCompile]
    public struct TrimPointsToRegionJob : IJobParallelForDefer
    {
        public float RegionOriginX;
        public float RegionOriginZ;
        public float RegionSizeX;
        public float RegionSizeZ;
        public float CarveMargin;

        public NativeArray<float2Point> Points;

        public void Execute(int i)
        {
            float2Point p = Points[i];

            bool inside =
                p.X >= RegionOriginX - CarveMargin &&
                p.X <= RegionOriginX + RegionSizeX + CarveMargin &&
                p.Z >= RegionOriginZ - CarveMargin &&
                p.Z <= RegionOriginZ + RegionSizeZ + CarveMargin;

            if (!inside)
            {
                p.Kind = HydrologyPointKind.None;
                Points[i] = p;
            }
        }
    }
}