using _Project.Features.ProceduralWorld.Domain.Hydrology;
using Unity.Collections;

namespace _Project.Features.ProceduralWorld.Infrastructure.Hydrology
{
    public static class RiverBranchExtensions
    {
        public static NativeSlice<RiverNode> GetNodes(
            this RiverBranch branch,
            NativeList<RiverNode> nodes)
        {
            return new NativeSlice<RiverNode>(
                nodes.AsArray(),
                branch.StartIndex,
                branch.Length);
        }
    }
}