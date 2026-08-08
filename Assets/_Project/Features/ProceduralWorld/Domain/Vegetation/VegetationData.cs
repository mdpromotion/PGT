using _Project.Features.ProceduralWorld.Domain.Chunks;
using Unity.Collections;

namespace _Project.Features.ProceduralWorld.Domain.Vegetation
{
    public sealed class VegetationData
    {
        public ChunkCoordinate Coordinate { get; }
        public NativeList<VegetationInstanceData> Instances { get; }

        public VegetationData(
            ChunkCoordinate coordinate,
            NativeList<VegetationInstanceData> instances)
        {
            Coordinate = coordinate;
            Instances = instances;
        }

        public void Dispose()
        {
            if (Instances.IsCreated)
                Instances.Dispose();
        }
    }
}