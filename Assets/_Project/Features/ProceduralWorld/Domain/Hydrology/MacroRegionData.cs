using Unity.Collections;
using Unity.Mathematics;

namespace _Project.Features.ProceduralWorld.Domain.Hydrology
{
    public sealed class MacroRegionData
    {
        public MacroRegionCoordinate Coordinate { get; }
        public int PaddedSize { get; }
        public float CellSize { get; }
        public float2 WorldOrigin { get; }

        public NativeArray<float> Heights;
        public NativeArray<sbyte> FlowDirection;
        public NativeArray<float> Accumulation;

        private bool _disposed;

        public MacroRegionData(
            MacroRegionCoordinate coordinate,
            int paddedSize,
            float cellSize,
            float2 worldOrigin)
        {
            Coordinate = coordinate;
            PaddedSize = paddedSize;
            CellSize = cellSize;
            WorldOrigin = worldOrigin;

            int count = paddedSize * paddedSize;
            Heights = new NativeArray<float>(count, Allocator.Persistent);
            FlowDirection = new NativeArray<sbyte>(count, Allocator.Persistent);
            Accumulation = new NativeArray<float>(count, Allocator.Persistent);
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            if (Heights.IsCreated) Heights.Dispose();
            if (FlowDirection.IsCreated) FlowDirection.Dispose();
            if (Accumulation.IsCreated) Accumulation.Dispose();
        }
    }
}