using Unity.Jobs;

namespace _Project.Features.ProceduralWorld.Infrastructure.Hydrology
{
    public readonly struct RegionFetchResult
    {
        public readonly HydrologyRegionData Data;
        public readonly JobHandle Handle;

        public RegionFetchResult(HydrologyRegionData data, JobHandle handle)
        {
            Data = data;
            Handle = handle;
        }
    }
}