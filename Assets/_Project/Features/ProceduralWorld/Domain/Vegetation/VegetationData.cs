using System.Collections.Generic;
using System.Linq;
using _Project.Features.ProceduralWorld.Domain.Chunks;

namespace _Project.Features.ProceduralWorld.Domain.Vegetation
{
    public class VegetationData
    {
        public ChunkCoordinate Coordinate { get; }
        public IReadOnlyList<VegetationLayerData> Layers { get; }

        public VegetationData(ChunkCoordinate coordinate, IReadOnlyList<VegetationLayerData> layers)
        {
            Coordinate = coordinate;
            Layers = layers;
        }

        public void Dispose()
        {
            foreach (var layer in Layers)
                layer.Dispose();
        }
    }
}