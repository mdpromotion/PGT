using Unity.Collections;
using Unity.Mathematics;

namespace _Project.Features.ProceduralWorld.Infrastructure.Vegetation
{
    public sealed class VegetationTileData
    {
        public VegetationTileCoordinate Coordinate { get; }
        public NativeList<float2> Points { get; }

        private bool _disposed;

        public VegetationTileData(VegetationTileCoordinate coordinate, NativeList<float2> points)
        {
            Coordinate = coordinate;
            Points = points;
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            if (Points.IsCreated) Points.Dispose();
        }
    }
}