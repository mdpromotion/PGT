using _Project.Features.ProceduralWorld.Infrastructure.Hydrology;
using Unity.Collections;

namespace _Project.Features.ProceduralWorld.Domain.Hydrology
{
    public struct HydrologyRegionData
    {
        [ReadOnly]
        public NativeArray<RiverSource> Sources;


        [ReadOnly]
        public NativeArray<RiverNode> Nodes;


        [ReadOnly]
        public NativeArray<RiverBranch> Branches;
    }
}