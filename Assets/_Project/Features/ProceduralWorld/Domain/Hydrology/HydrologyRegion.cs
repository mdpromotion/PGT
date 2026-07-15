using _Project.Features.ProceduralWorld.Infrastructure.Hydrology;
using Unity.Collections;

namespace _Project.Features.ProceduralWorld.Domain.Hydrology
{
    public enum HydrologyRegionState
    {
        Created,
        GeneratingSources,
        Ready
    }



    public sealed class HydrologyRegion
    {
        public HydrologyRegionCoordinate Coordinate { get; }


        public HydrologyRegionState State { get; private set; }



        public NativeList<RiverSource> Sources;


        public NativeList<RiverNode> Nodes;


        public NativeList<RiverBranch> Branches;
        
        public int SourceCount =>
            Sources.Length;


        public HydrologyRegion(
            HydrologyRegionCoordinate coordinate,
            Allocator allocator)
        {
            Coordinate = coordinate;


            Sources =
                new NativeList<RiverSource>(
                    allocator);


            Nodes =
                new NativeList<RiverNode>(
                    allocator);


            Branches =
                new NativeList<RiverBranch>(
                    allocator);


            State =
                HydrologyRegionState.Created;
        }



        public void SetGeneratingSources()
        {
            State =
                HydrologyRegionState.GeneratingSources;
        }



        public void SetReady()
        {
            State =
                HydrologyRegionState.Ready;
        }



        public void Dispose()
        {
            if (Branches.IsCreated)
                Branches.Dispose();


            if (Nodes.IsCreated)
                Nodes.Dispose();


            if (Sources.IsCreated)
                Sources.Dispose();
        }
    }
}